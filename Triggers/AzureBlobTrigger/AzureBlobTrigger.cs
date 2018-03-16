// Copyright 2016-2040 Nino Crudele
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#region Usings

using System.Diagnostics;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace SnapGate.Framework.AzureBlobTrigger
{
    /// <summary>
    ///     The azure blob trigger.
    /// </summary>
    [TriggerContract("{3BADD8A0-211B-4C57-806B-8C0453EB637B}", "Azure Blob Trigger", "Azure Blob Trigger", true, true,
        false)]
    public class AzureBlobTrigger : ITriggerType
    {
        /// <summary>
        ///     Gets or sets the storage account.
        /// </summary>
        [TriggerPropertyContract("StorageAccount", "Azure StorageAccount")]
        public string StorageAccount { get; set; }

        /// <summary>
        ///     Gets or sets the blob container.
        /// </summary>
        [TriggerPropertyContract("BlobContainer", "Azure Blob Container")]
        public string BlobContainer { get; set; }

        /// <summary>
        ///     Gets or sets the blob block reference.
        /// </summary>
        [TriggerPropertyContract("BlobBlockReference", "Azure Blob BlockReference")]
        public string BlobBlockReference { get; set; }

        public AutoResetEvent WaitHandle { get; set; }

        public string SupportBag { get; set; }

        [TriggerPropertyContract("Syncronous", "Trigger Syncronous")]
        public bool Syncronous { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action trigger.
        /// </summary>
        public ActionTrigger ActionTrigger { get; set; }

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        [TriggerPropertyContract("DataContext", "Trigger Default Main Data")]
        public byte[] DataContext { get; set; }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="actionTrigger">
        ///     The set event action trigger.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        [TriggerActionContract("{1FE5C5DA-F856-458C-8D67-0BF3F5997583}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(StorageAccount);
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve a reference to a container. 
                var container = blobClient.GetContainerReference(BlobContainer);

                // Create the container if it doesn't already exist.
                container.CreateIfNotExists();
                container.SetPermissions(
                    new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});

                var blockBlob = container.GetBlockBlobReference(BlobBlockReference);

                try
                {
                    blockBlob.FetchAttributes();
                    var fileByteLength = blockBlob.Properties.Length;
                    var blobContent = new byte[fileByteLength];
                    for (var i = 0; i < fileByteLength; i++)
                    {
                        blobContent[i] = 0x20;
                    }

                    blockBlob.DownloadToByteArray(blobContent, 0);
                    blockBlob.Delete();
                    DataContext = blobContent;
                    actionTrigger(this, context);
                }
                catch
                {
                    // ignored
                }

                return null;
            }
            catch
            {
                // ignored
                return null;
            }
        }

        /// <summary>
        ///     The my on entry written.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        public void MyOnEntryWritten(object source, EntryWrittenEventArgs e)
        {
            DataContext = EncodingDecoding.EncodingString2Bytes(e.Entry.Message);
            ActionTrigger(this, Context);
        }
    }
}