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
using System.ServiceProcess;
using System.Threading;
using SnapGate.Framework.Base;
using SnapGate.Framework.Engine;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.NTService
{
    /// <summary>
    ///     Component that represents the Windows Service.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public partial class NTWindowsService : ServiceBase
    {
        /// <summary>
        ///     Starts the core Engine.
        /// </summary>
        public static void StartEngine()
        {
            try
            {
                // Start NT service
                Trace.WriteLine("LogEventUpStream - Initialization--Start Engine.");
                Trace.WriteLine("Initialization--Start Engine.");
                LogEngine.Init();
                Trace.WriteLine("LogEventUpStream - StartEventEngine.");
                CoreEngine.StartEventEngine(null);
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
                Thread.Sleep(ConfigurationBag.Configuration.WaitTimeBeforeRestarting);
                Environment.Exit(0);
            }
        }

        // StartEngine

        /// <summary>
        ///     Called when the Windows Service starts.
        /// </summary>
        /// <param name="args">
        ///     The arguments.
        /// </param>
        protected override void OnStart(string[] args)
        {
            try
            {
                // ******************************************
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Instance {ServiceName} engine starting.",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    null,
                    Constant.LogLevelInformation);

                var engineThreadProcess = new Thread(StartEngine);

                engineThreadProcess.Start();

                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Instance {ServiceName} engine started.",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    null,
                    Constant.LogLevelInformation);
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
                Thread.Sleep(ConfigurationBag.Configuration.WaitTimeBeforeRestarting);
                Environment.Exit(0);
            }
        }

        // OnStart

        /// <summary>
        ///     Called when Windows Service stops.
        /// </summary>
        protected override void OnStop()
        {
            CoreEngine.StopEventEngine();
        }

        // OnStop
    } // NTWindowsService
} // namespace