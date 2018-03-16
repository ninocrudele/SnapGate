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
using System.Diagnostics;
using System.Reflection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Storage;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.Storage
{
    /// <summary>
    ///     Main persistent provider.
    /// </summary>
    [DevicePersistentProviderContract("{53158DA4-EAEA-4D8A-90C8-81A66F7A0F74}", "DevicePersistentProvider",
        "Device Persistent Provider for Azure")]
    public class BlobDevicePersistentProvider : IDevicePersistentProvider
    {
        public void PersistEventToStorage(byte[] messageBody, string messageId)
        {
            try
            {
                var storageAccountName = ConfigurationBag.Configuration.GroupStorageAccountName;
                var storageAccountKey = ConfigurationBag.Configuration.GroupStorageAccountKey;
                var connectionString =
                    $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve a reference to a container. 
                var container =
                    blobClient.GetContainerReference(ConfigurationBag.Configuration.GroupStorageAccountName +
                                                     "gcstorage");

                // Create the container if it doesn't already exist.
                container.CreateIfNotExists();
                container.SetPermissions(
                    new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});

                // Create the messageid reference
                var blockBlob = container.GetBlockBlobReference(messageId);
                blockBlob.UploadFromByteArray(messageBody, 0, messageBody.Length);
                Trace.WriteLine(
                    "Event persisted -  Consistency Transaction Point created.");
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
            }
        }

        public byte[] PersistEventFromStorage(string messageId)
        {
            try
            {
                var storageAccountName = ConfigurationBag.Configuration.GroupStorageAccountName;
                var storageAccountKey = ConfigurationBag.Configuration.GroupStorageAccountKey;
                var connectionString =
                    $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve a reference to a container. 
                var container =
                    blobClient.GetContainerReference(ConfigurationBag.Configuration.GroupStorageAccountName +
                                                     "gcstorage");

                // Create the container if it doesn't already exist.
                container.CreateIfNotExists();
                container.SetPermissions(
                    new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});

                // Create the messageid reference
                var blockBlob = container.GetBlockBlobReference(messageId);

                blockBlob.FetchAttributes();
                var msgByteLength = blockBlob.Properties.Length;
                var msgContent = new byte[msgByteLength];
                for (var i = 0; i < msgByteLength; i++)
                {
                    msgContent[i] = 0x20;
                }

                blockBlob.DownloadToByteArray(msgContent, 0);

                Trace.WriteLine(
                    "Event persisted recovered -  Consistency Transaction Point restored.");

                return msgContent;
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
                return null;
            }
        }
    }
}