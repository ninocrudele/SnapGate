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
using System.Security;
using System.Text;
using System.Threading;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.RunProcess
{
    /// <summary>
    ///     The Run Process trigger.
    /// </summary>
    [TriggerContract("{B7226137-DFF7-44BD-9E48-42B5AC7AF730}", "RunProcess", "Run a specific application or batch.",
        false, true, false)]
    public class RunProcess : ITriggerType
    {
        /// <summary>
        ///     Gets or sets the process file to execute.
        /// </summary>
        [TriggerPropertyContract("ProcessPathFileName", "Gets or sets the process file to execute.")]
        public string ProcessPathFileName { get; set; }

        /// <summary>
        ///     Gets or sets how the process nee to start.
        /// </summary>
        [TriggerPropertyContract("ProcessStyle",
            "Process style behaviour, look for ProcessWindowStyle in system.Process.")]
        public string ProcessStyle { get; set; }

        /// <summary>
        ///     Gets or sets the polling time.
        /// </summary>
        [TriggerPropertyContract("ExecuteEventAfterRun", "If need to execute the event after run the process.")]
        public bool ExecuteEventAfterRun { get; set; }

        /// <summary>
        ///     Gets or sets the polling time.
        /// </summary>
        [TriggerPropertyContract("AlwaysRun", "Run again in case of unexpected shutdown.")]
        public bool AlwaysRun { get; set; }

        /// <summary>
        ///     Gets or sets the Domain.
        /// </summary>
        [TriggerPropertyContract("Domain", "Domain to run the process.")]
        public string Domain { get; set; }

        /// <summary>
        ///     Gets or sets the polling time.
        /// </summary>
        [TriggerPropertyContract("User", "Username to run the process.")]
        public string User { get; set; }

        /// <summary>
        ///     Gets or sets the polling time.
        /// </summary>
        [TriggerPropertyContract("Password", "Password to run the process.")]
        public string Password { get; set; }

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
        [TriggerActionContract("{CABFAC55-56F1-4863-84C2-29D01AADA834}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                using (new Impersonation(Domain, User, Password))
                {
                    Process process = new Process();
                    // Configure the process using the StartInfo properties.
                    //process.StartInfo.FileName = "C:\\opt\\spg\\apache-servicemix-6.0.0\\bin\\servicemix.bat";

                    process.StartInfo.Arguments = "";
                    switch (ProcessStyle)
                    {
                        case "Hidden":
                            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            break;
                        case "Maximized":
                            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                            break;
                        case "Minimized":
                            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                            break;
                        case "Normal":
                            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                            break;
                        default:
                            break;
                    }

                    ProcessStartInfo processStartInfo = new ProcessStartInfo(ProcessPathFileName);
                    //processStartInfo.RedirectStandardOutput= true;
                    //processStartInfo.UseShellExecute = false;
                    processStartInfo.FileName = ProcessPathFileName;
                    processStartInfo.WorkingDirectory = Path.GetDirectoryName(ProcessPathFileName);
                    //processStartInfo.UserName = User;
                    //SecureString secureString = GetSecureString(Password);
                    //processStartInfo.Password = secureString;

                    process.StartInfo = processStartInfo;

                    if (AlwaysRun)
                    {
                        while (true)
                        {
                            process.Start();
                            DataContext = Encoding.UTF8.GetBytes("nop");
                            if (ExecuteEventAfterRun)
                                actionTrigger(this, context);
                            else
                                actionTrigger(null, null);

                            LogEngine.WriteLog(ConfigurationBag.EngineName,
                                $"The process {ProcessPathFileName} started.",
                                Constant.LogLevelError,
                                Constant.TaskCategoriesError,
                                null,
                                Constant.LogLevelInformation);
                            process.WaitForExit();
                            Thread.Sleep(1000);
                            //this.Context.BubblingConfiguration.Events[0].IdComponent
                        }
                    }

                    process.Start();
                    DataContext = Encoding.UTF8.GetBytes("nop");
                    if (ExecuteEventAfterRun)
                        actionTrigger(this, context);
                    else
                        actionTrigger(null, null);

                    LogEngine.WriteLog(ConfigurationBag.EngineName,
                        $"The process {ProcessPathFileName} started.",
                        Constant.LogLevelError,
                        Constant.TaskCategoriesError,
                        null,
                        Constant.LogLevelInformation);

                    process.WaitForExit();
                }

                return null;
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(ConfigurationBag.EngineName,
                    $"Critical Error in RunProcess Trigger executing process {ProcessPathFileName}.",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
                actionTrigger(this, null);
                return null;
            }
        }

        private SecureString GetSecureString(string password)
        {
            SecureString sc = new SecureString();

            foreach (var c in password)
            {
                sc.AppendChar(c);
            }

            return sc;
        }
    }
}