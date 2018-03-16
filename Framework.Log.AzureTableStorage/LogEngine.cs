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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Log;

#endregion

namespace SnapGate.Framework.Log.AzureTableStorage
{
    /// <summary>
    ///     The log engine, simple version.
    /// </summary>
    [LogContract("{CE541CB7-94CD-4421-B6C4-26FBC3088FF9}", "LogEngine", "Azure Table Storage Log System")]
    public class LogEngine : ILogEngine
    {
        private TableBatchOperation batchOperation;
        private CloudTable tableGlobal;

        /// <summary>
        ///     Initialize log.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool InitLog()
        {
            var storageAccountName = ConfigurationBag.Configuration.GroupStorageAccountName;
            var storageAccountKey = ConfigurationBag.Configuration.GroupStorageAccountKey;
            var connectionString =
                $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            tableGlobal = tableClient.GetTableReference("SnapGategloballog");
            tableGlobal.CreateIfNotExists();
            batchOperation = new TableBatchOperation();
            return true;
        }

        /// <summary>
        ///     The write log.
        /// </summary>
        /// <param name="logMessage">
        ///     The log message.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool WriteLog(LogMessage logMessage)
        {
            //TableOperation insertOperation = TableOperation.Insert(logMessage);
            //tableGlobal.Execute(insertOperation);

            batchOperation.Insert(logMessage);
            return true;
        }

        public void Flush()
        {
            try
            {
                // Execute the insert operation.
                tableGlobal.ExecuteBatch(batchOperation);
                batchOperation.Clear();
            }
            catch (Exception ex)
            {
                Log.LogEngine.DirectEventViewerLog(
                    $"Error in SnapGate.Framework.Log.AzureTableStorage component - {ex.Message}", 1);
            }
        }
    }
}