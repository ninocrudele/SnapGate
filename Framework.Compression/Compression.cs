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

using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using SnapGate.Framework.Base;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.CompressionLibrary
{
    public static class Helpers
    {
        /// <summary>
        ///     Compress a folder
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>a byte array</returns>
        public static byte[] CreateFromDirectory(string directoryPath)
        {
            string zipFolderFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.zip");

            try
            {
                ZipFile.CreateFromDirectory(directoryPath, zipFolderFile, CompressionLevel.Fastest, true);
                byte[] fileStream = File.ReadAllBytes(zipFolderFile);
                File.Delete(zipFolderFile);
                return fileStream;
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
                return null;
            }
        }

        /// <summary>
        ///     Decompress byte stream
        /// </summary>
        /// <param name="fileContent"></param>
        public static void CreateFromBytearray(byte[] fileContent, string unzipFolder)
        {
            string unzipFolderFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.zip");

            //Delete current Sync folder
            if (Directory.Exists(unzipFolder))
                DeleteFilesAndFoldersRecursively(unzipFolder);
            try
            {
                File.WriteAllBytes(unzipFolderFile, fileContent);
                ZipFile.ExtractToDirectory(unzipFolderFile, unzipFolder);
                File.Delete(unzipFolderFile);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
            }
        }

        public static void DeleteFilesAndFoldersRecursively(string targetDir)
        {
            foreach (string file in Directory.GetFiles(targetDir))
            {
                File.Delete(file);
            }

            foreach (string subDir in Directory.GetDirectories(targetDir))
            {
                DeleteFilesAndFoldersRecursively(subDir);
            }

            Thread.Sleep(1); // This makes the difference between whether it works or not. Sleep(0) is not enough.
            Directory.Delete(targetDir);
        }
    }
}