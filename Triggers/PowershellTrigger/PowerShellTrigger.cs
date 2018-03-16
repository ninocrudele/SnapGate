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
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.PowerShellTrigger
{
    /// <summary>
    ///     TODO The power shell trigger.
    /// </summary>
    [TriggerContract("{18BB5E65-23A2-4743-8773-32F039AA3D16}", "PowerShell Trigger",
        "Execute a trigger write in Powerhell script", true, true, false)]
    public class PowerShellTrigger : ITriggerType
    {
        /// <summary>
        ///     Gets or sets the script.
        /// </summary>
        [TriggerPropertyContract("Script", "Script to execute")]
        public string Script { get; set; }

        /// <summary>
        ///     Gets or sets the script file.
        /// </summary>
        [TriggerPropertyContract("ScriptFile", "Script from file")]
        public string ScriptFile { get; set; }

        /// <summary>
        /// If polling is required
        /// </summary>
        [TriggerPropertyContract("Polling", "If polling is required")]
        public bool Polling { get; set; }

        /// <summary>
        /// Polling time in milliseconds
        /// </summary>
        [TriggerPropertyContract("PollingTime", "Polling time in milliseconds")]
        public int PollingTime { get; set; }

        /// <summary>
        ///     Gets or sets the message properties.
        /// </summary>
        [EventPropertyContract("MessageProperties", "MessageProperties")]
        public string MessageProperties { get; set; }

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
        /// <exception cref="Exception">
        /// </exception>
        [TriggerActionContract("{78B0F3C0-96D6-4DF6-83CD-C282FB6C6D54}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            var script = string.Empty;

            if (ScriptFile != string.Empty)
                script = File.ReadAllText(ScriptFile);
            if (Script != string.Empty)
                script = Script;
            if (ScriptFile != string.Empty && Script != string.Empty)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name} - PowerShell trigger error: ScriptFile and Script variable both empty.",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    null,
                    Constant.LogLevelError);
                return null;
            }

            var powerShellScript = PowerShell.Create();
            powerShellScript.AddScript(script);

            powerShellScript.AddParameter("DataContext", DataContext);
            while (true)
            {
                powerShellScript.Invoke();
                if (powerShellScript.HadErrors)
                {
                    var sb = new StringBuilder();
                    foreach (var error in powerShellScript.Streams.Error)
                    {
                        sb.AppendLine(error.Exception.Message);
                    }

                    throw new Exception(sb.ToString());
                }

                var outVar = powerShellScript.Runspace.SessionStateProxy.PSVariable.GetValue("DataContext");

                if (outVar != null && outVar.ToString() != string.Empty)
                {
                    try
                    {
                        var po = (PSObject) outVar;
                        var logEntry = po.BaseObject as EventLogEntry;
                        if (logEntry != null)
                        {
                            var ev = logEntry;
                            DataContext = EncodingDecoding.EncodingString2Bytes(ev.Message);
                        }
                        else
                        {
                            DataContext = EncodingDecoding.EncodingString2Bytes(outVar.ToString());
                        }
                    }
                    catch
                    {
                        StringBuilder sb = new StringBuilder();
                        // if multiple pso
                        var results = (object[]) outVar;

                        foreach (var pos in results)
                        {
                            var po = (PSObject) pos;
                            sb.AppendLine(po.ToString());
                        }

                        DataContext = EncodingDecoding.EncodingString2Bytes(sb.ToString());
                        actionTrigger(this, context);
                    }
                }

                if (!Polling)
                {
                    return null;
                }
                else
                {
                    Thread.Sleep(PollingTime);
                }
            }
        }
    }
}