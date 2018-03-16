using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTrigger
{
    public class FileTrigger
    {
        public static int MaximumBatchSize { get; set; }
        public static int MaximumNumberOfFiles { get; set; }
        public static int MaxFileSize { get; set; }
        
        public static string Directory { get; set; }
        public static string FileMask { get; set; }
        public static string WorkInProgress { get; set; }

        
        //  The algorithm implemented here splits the list of files according to the
        //  batch tuning parameters (number of bytes and number of files) because the
        //  list is randomly ordered it is possible to have non-optimal batches. It would
        //  be a slight optimization to order by increasing size and then cut the batches.
        private void PickupFilesAndSubmit()
        {
            Trace.WriteLine("[DotNetFileReceiverEndpoint] PickupFilesAndSubmit called");

            int maxBatchSize = MaximumBatchSize;
            int maxNumberOfFiles = MaximumNumberOfFiles;

            List<BatchMessage> files = new List<BatchMessage>();
            long bytesInBatch = 0;

            DirectoryInfo di = new DirectoryInfo(Directory);
            FileInfo[] items = di.GetFiles(FileMask);

            Trace.WriteLine(string.Format("[DotNetFileReceiverEndpoint] Found {0} files.", items.Length));
            foreach (FileInfo item in items)
            {
                //  only consider files that are not read only
                if (FileAttributes.ReadOnly == (FileAttributes.ReadOnly & item.Attributes))
                    continue;

                //  only download files that are less than the configured max file size
                if (false == CheckMaxFileSize(item))
                    continue;

                string fileName = Path.Combine(Directory, item.Name);
                string renamedFileName;
                if (WorkInProgress.Length > 0)
                    renamedFileName = fileName + WorkInProgress;
                else
                    renamedFileName = null;

                // If we couldn't lock the file, just move onto the next file
                byte[] msg = CreateMessage(fileName, renamedFileName);
                if (null == msg)
                    continue;

                if (null == renamedFileName)
                    files.Add(new BatchMessage(msg, fileName, BatchOperationType.Submit));
                else
                    files.Add(new BatchMessage(msg, renamedFileName, BatchOperationType.Submit));

                //  keep a running total for the current batch
                bytesInBatch += item.Length;

                //  zero for the value means infinite 
                bool fileCountExceeded = ((0 != maxNumberOfFiles) && (files.Count >= maxNumberOfFiles));
                bool byteCountExceeded = ((0 != maxBatchSize) && (bytesInBatch > maxBatchSize));

                if (fileCountExceeded || byteCountExceeded)
                {
                    //  check if we have been asked to stop - if so don't start another batch
                    if (this.controlledTermination.TerminateCalled)
                        return;

                    //  execute the batch
                    SubmitFiles(files);

                    //  reset the running totals
                    bytesInBatch = 0;
                    files.Clear();
                }
            }

            //  check if we have been asked to stop - if so don't start another batch
            if (this.controlledTermination.TerminateCalled)
                return;

            //  end of file list - one final batch to do
            if (files.Count > 0)
                SubmitFiles(files);
        }

        /// <summary>
        /// Given a List of Messages submit them to BizTalk for processing
        /// </summary>
        private void SubmitFiles(List<BatchMessage> files)
        {
            if (files == null || files.Count == 0) throw new ArgumentException("SubmitFiles was called with an empty list of Files");

            Trace.WriteLine(string.Format("[DotNetFileReceiverEndpoint] SubmitFiles called. Submitting a batch of {0} files.", files.Count));

            //This class is used to track the files associated with this ReceiveBatch
            BatchInfo batchInfo = new BatchInfo(files);
            using (ReceiveBatch batch = new ReceiveBatch(this.transportProxy, this.controlledTermination, batchInfo.OnBatchComplete, this.properties.MaximumNumberOfFiles))
            {
                foreach (BatchMessage file in files)
                {
                    // submit file to batch
                    batch.SubmitMessage(file.Message, file.UserData);
                }

                batch.Done(null);
            }
        }

        private bool CheckMaxFileSize(FileInfo item)
        {
            long fileSizeBytes = item.Length;
            if (MaxFileSize > 0)
            {
                long maxFileSizeBytes = 1024 * 1024 * MaxFileSize;
                return (fileSizeBytes <= maxFileSizeBytes);
            }
            else
                return true;
        }

        /// <summary>
        /// Create a BizTalk message given the name of a file on disk optionally renaming
        /// the file while the message is being submitted into BizTalk.
        /// </summary>
        /// <param name="srcFilePath">The File to create the message from</param>
        /// <param name="renamedFileName">Optional, if specified the file will be renamed to this value.</param>
        /// <returns>The message to be submitted to BizTalk.</returns>
        private byte[] CreateMessage(string srcFilePath, string renamedFileName)
        {
            byte[] byteContent;
            bool renamed = false;

            // Open the file
            try
            {
                if (!String.IsNullOrEmpty(renamedFileName))
                {
                    Trace.WriteLine("[DotNetFileReceiverEndpoint] Renaming file " + srcFilePath);
                    File.Move(srcFilePath, renamedFileName);
                    renamed = true;
                    byteContent = File.ReadAllBytes(renamedFileName);
                }
                else
                {
                    byteContent = File.ReadAllBytes(srcFilePath);
                }
            }
            catch (Exception)
            {
                // If we renamed the file, rename it back
                if (renamed)
                    File.Move(renamedFileName, srcFilePath);

                return null;
            }

            return byteContent;
        }

    }
}
