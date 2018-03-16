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
using System.Diagnostics.CodeAnalysis;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Serialization;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace SnapGate.Framework.BulksqlServerTrigger
{
    /// <summary>
    ///     The bulksql server trigger.
    /// </summary>
    [TriggerContract("{9A989BD1-C8DE-4FC1-B4BA-02E7D8A4AD76}", "SQL Server Bulk Trigger",
        "Execute a bulk insert between databases.", false,
        true, true)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    public class BulksqlServerTrigger : ITriggerType
    {
        /// <summary>
        ///     Gets or sets the table name.
        /// </summary>
        [TriggerPropertyContract("TableName", "TableName")]
        public string TableName { get; set; }

        /// <summary>
        ///     Gets or sets the bulk select query.
        /// </summary>
        [TriggerPropertyContract("BulkSelectQuery", "BulkSelectQuery")]
        public string BulkSelectQuery { get; set; }

        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        [TriggerPropertyContract("ConnectionString", "ConnectionString")]
        public string ConnectionString { get; set; }

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
        [TriggerActionContract("{C55D1D0A-B4B4-4FF0-B41F-38CE0C7A522C}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                Context = context;
                ActionTrigger = actionTrigger;

                using (var sourceConnection = new SqlConnection(ConnectionString))
                {
                    sourceConnection.Open();

                    // Get data from the source table as a SqlDataReader.
                    var commandSourceData = new SqlCommand(BulkSelectQuery, sourceConnection);
                    var dataTable = new DataTable();
                    var dataAdapter = new SqlDataAdapter(commandSourceData);
                    dataAdapter.Fill(dataTable);
                    DataContext = Serialization.DataTableToByteArray(dataTable);
                    actionTrigger(this, context);
                }

                return null;
            }
            catch (Exception)
            {
                // ignored
                return null;
            }
        }
    }
}