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

using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;

#endregion

namespace SnapGate.Framework.AzureBlobEvent
{
    /// <summary>
    ///     Handles the Azure Blob event.
    /// </summary>
    [EventContract("{C185F004-62E4-45A4-97B1-BD0D382FFE33}", "Azure Blob Event", "Show a messagebox", true)]
    public class AzureBlobEvent : IEventType
    {
        /// <summary>
        ///     Gets or sets the Azure storage account.
        /// </summary>
        /// <value>
        ///     The Azure storage account.
        /// </value>
        [EventPropertyContract("StorageAccount", "Azure StorageAccount")]
        public string StorageAccount { get; set; }

        /// <summary>
        ///     Gets or sets the Azure BLOB container.
        /// </summary>
        /// <value>
        ///     The Azure BLOB container.
        /// </value>
        [EventPropertyContract("BlobContainer", "Azure Blob Container")]
        public string BlobContainer { get; set; }

        /// <summary>
        ///     Gets or sets the Azure BLOB block reference.
        /// </summary>
        /// <value>
        ///     The Azure BLOB block reference.
        /// </value>
        [EventPropertyContract("BlobBlockReference", "Azure Blob BlockReference")]
        public string BlobBlockReference { get; set; }

        /// <summary>
        ///     Gets or sets the event context.
        /// </summary>
        /// <value>
        ///     The context.
        /// </value>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action event.
        /// </summary>
        /// <value>
        ///     The set event action event.
        /// </value>
        public ActionEvent ActionEvent { get; set; }

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        /// <value>
        ///     The data context.
        /// </value>
        [EventPropertyContract("DataContext", "Event Default Main Data")]
        public byte[] DataContext { get; set; }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="actionEvent">
        ///     The set event action event.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        [EventActionContract("{346FC437-8464-4566-8AD6-A7E4B29A7EBC}", "Main action", "Main action description")]
        public async Task Execute(ActionEvent actionEvent, ActionContext context)
        {
            try
            {
                Context = context;
                ActionEvent = actionEvent;

                var storageAccount = CloudStorageAccount.Parse(StorageAccount);
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve a reference to a container. 
                var container = blobClient.GetContainerReference(BlobContainer);

                // Create the container if it doesn't already exist.
                container.CreateIfNotExists();
                container.SetPermissions(
                    new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});

                var blockBlob = container.GetBlockBlobReference(BlobBlockReference);
                blockBlob.UploadFromByteArray(DataContext, 0, DataContext.Length);

                actionEvent(this, context);
                return;
            }
            catch
            {
                return;
            }
        } // Execute
    } // AzureBlobEvent
} // namespace