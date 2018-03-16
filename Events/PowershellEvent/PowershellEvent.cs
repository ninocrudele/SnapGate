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
using System.Management.Automation;
using System.Threading.Tasks;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;

#endregion

namespace SnapGate.Framework.PowershellEvent
{
    /// <summary>
    ///     The PowerShell event.
    /// </summary>
    [EventContract("{F9A0B69C-64D3-4120-A52D-09D2E014EA91}", "Execute a Powershell Event", "Execute a Powershell Event",
        true)]
    public class PowershellEvent : IEventType
    {
        /// <summary>
        ///     Gets or sets the script event.
        /// </summary>
        [EventPropertyContract("ScriptEvent", "Script to execute")]
        public string ScriptEvent { get; set; }

        /// <summary>
        ///     Gets or sets the script file event.
        /// </summary>
        [EventPropertyContract("ScriptFileEvent", "Script from file")]
        public string ScriptFileEvent { get; set; }

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
        [EventActionContract("{979A5EE0-C029-4518-98C2-CFB4526F2C86}", "Main action", "Main action description")]
        public async Task Execute(ActionEvent actionEvent, ActionContext context)
        {
            try
            {
                var script = string.Empty;
                if (!string.IsNullOrEmpty(ScriptFileEvent))
                {
                    script = File.ReadAllText(ScriptFileEvent);
                }
                else
                {
                    script = ScriptEvent;
                }

                var powerShellScript = PowerShell.Create();
                powerShellScript.AddScript(script);

                // TODO 1020
                powerShellScript.AddParameter("DataContext", DataContext);
                powerShellScript.Invoke();
                var outVar = powerShellScript.Runspace.SessionStateProxy.PSVariable.GetValue("DataContext");
                if (outVar != null)
                {
                    DataContext = EncodingDecoding.EncodingString2Bytes(outVar.ToString());
                    actionEvent(this, context);
                }

                return;
                // TODO 1030
            }
            catch (Exception)
            {
                // ignored
                return;
            }
        }
    }
}