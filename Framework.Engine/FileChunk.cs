using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapGate.Framework.Engine
{
    public enum FileChunkTypeEnum
                    {
                        AssemblyFilesSnapGate,
                        AssemblyFilesTrigger,
                        AssemblyFilesEvent,
                        AssemblyFilesComponent,
                        PropertyTriggersFile,
                        PropertyEventsFile,
                        PropertyChainsFile,
                        PropertyComponentsFile

                    }



    [Serializable]
    public class FileChunk
    {
        public FileChunkTypeEnum FileChunkType { get; set; }
        public string FilePathName { get; set; }
        public byte[] FileContent { get; set; }

        public FileChunk(FileChunkTypeEnum fileChunkType,string filePathName,  byte[] fileContent)
        {
            FileChunkType = fileChunkType;
            FilePathName = filePathName;
            FileContent = fileContent;
        }
    }
}
