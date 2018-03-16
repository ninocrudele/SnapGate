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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace SnapGate.Framework.SqlServerTrigger
{
    /// <summary>
    ///     The SQL server trigger.
    /// </summary>
    [TriggerContract("{7920EE0F-CAC8-4ABB-82C2-1C69351EDD28}", "Sql Server Trigger",
        "Execute a Sql query or stored procedure.",
        false, true, false)]
    public class SqlServerTrigger : ITriggerType
    {
        /// <summary>
        ///     Gets or sets the SQL query.
        /// </summary>
        [TriggerPropertyContract("SqlQuery", "Select Command [Select * from or EXEC Stored precedure name]")]
        public string SqlQuery { get; set; }

        /// <summary>
        ///     Gets or sets the polling time.
        /// </summary>
        [TriggerPropertyContract("PollingTime", "Polling time.")]
        public int PollingTime { get; set; }

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
        [TriggerActionContract("{7BA7B689-6A1D-4FF6-87B3-720F9A723FB8}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                while (true)
                {
                    using (var myConnection = new SqlConnection(ConnectionString))
                    {
                        var selectCommand = new SqlCommand(SqlQuery, myConnection);
                        myConnection.Open();
                        XmlReader readerResult = null;
                        try
                        {
                            readerResult = selectCommand.ExecuteXmlReader();
                            readerResult.Read();
                        }
                        catch (Exception)
                        {
                        }

                        if (!readerResult.EOF)
                        {
                            var xdoc = new XmlDocument();
                            xdoc.Load(readerResult);
                            if (xdoc.OuterXml != string.Empty)
                            {
                                DataContext = EncodingDecoding.EncodingString2Bytes(xdoc.OuterXml);
                                //myConnection.Close();
                                actionTrigger(this, context);
                            }
                        }

                        Thread.Sleep(PollingTime);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error in SqlServerTrigger - {ex.Message}");
                return null;
            }
        }
    }
}