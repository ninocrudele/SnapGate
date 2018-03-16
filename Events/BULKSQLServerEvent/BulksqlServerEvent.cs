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
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Serialization;

#endregion

namespace SnapGate.Framework.BulksqlServerEvent
{
    /// <summary>
    ///     The bulksql server event.
    /// </summary>
    [EventContract("{767D579B-986B-47B1-ACDF-46738434043F}", "BulksqlServerEvent Event",
        "Receive a Sql Server recordset to perform a bulk insert.",
        true)]
    public class BulksqlServerEvent : IEventType
    {
        /// <summary>
        ///     Gets or sets the table name destination.
        /// </summary>
        [EventPropertyContract("TableNameDestination", "TableName")]
        public string TableNameDestination { get; set; }

        /// <summary>
        ///     Gets or sets the bulk select query destination.
        /// </summary>
        [EventPropertyContract("BulkSelectQueryDestination", "BulkSelectQueryDestination")]
        public string BulkSelectQueryDestination { get; set; }

        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        [EventPropertyContract("ConnectionString", "ConnectionString")]
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action event.
        /// </summary>
        public ActionEvent ActionEvent { get; set; }

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
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
        [EventActionContract("{F469BD5B-B352-40D6-BD33-591EF96E8F6C}", "Main action", "Main action description")]
        public async Task Execute(ActionEvent actionEvent, ActionContext context)
        {
            try
            {
                using (var destinationConnection = new SqlConnection(ConnectionString))
                {
                    destinationConnection.Open();
                    using (var bulkCopy = new SqlBulkCopy(ConnectionString))
                    {
                        bulkCopy.DestinationTableName = TableNameDestination;
                        try
                        {
                            object obj = Serialization.ByteArrayToDataTable(DataContext);
                            var dataTable = (DataTable) obj;

                            // Write from the source to the destination.
                            bulkCopy.WriteToServer(dataTable);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                }

                actionEvent(this, context);
                return;
            }
            catch
            {
                // ignored
                return;
            }
        }
    }
}