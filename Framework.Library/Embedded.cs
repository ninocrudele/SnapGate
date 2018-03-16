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
using System.Linq;
using System.Reflection;
using System.Threading;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Engine;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.Library
{
    /// <summary>
    ///     The embedded point.
    /// </summary>
    public class Embedded
    {
        public delegate void SetEventActionEventEmbedded(IEventType _this, ActionContext context);

        /// <summary>
        /// </summary>
        public static bool engineLoaded;

        // Global Action Events
        /// <summary>
        ///     The delegate action event.
        /// </summary>
        private static ActionEvent _delegate;

        private static byte[] _syncronousDataContext;

        /// <summary>
        ///     Used internally by the embedded
        /// </summary>
        public static SetEventActionEventEmbedded setEventActionEventEmbedded { get; set; }

        public static AutoResetEvent eventStop { get; set; }
        public static SyncAsyncEventAction SyncAsyncEventAction { get; set; }


        /// <summary>
        ///     Handles the ProcessExit event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void CurrentDomainProcessExit(object sender, EventArgs e)
        {
            CoreEngine.StopEventEngine();
        } // CurrentDomain_ProcessExit

        public static void InitializeEngine()
        {
            Thread t = new Thread(StartEngine);
            t.Start();
            EngineStartedAsync();
        }

        public static void StartMinimalEngine()
        {
            try
            {
                ConfigurationBag.LoadConfiguration();
                LogEngine.Init();
                Trace.WriteLine(
                    $"Version {Assembly.GetExecutingAssembly().GetName().Version}");

                Trace.WriteLine("--SnapGate Sevice Initialization--Start Engine.");
                // delegateActionEvent = delegateActionEventEmbedded;
                CoreEngine.StartEventEngine(null);
                engineLoaded = true;
            }
            catch (NotImplementedException ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    "Error in " + MethodBase.GetCurrentMethod().Name,
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    "Error in " + MethodBase.GetCurrentMethod().Name,
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
                Environment.Exit(0);
            } // try/catch
            finally
            {
                //Spool log queues
                if (LogEngine.QueueAbstractMessage != null)
                {
                    LogEngine.QueueAbstractMessageOnPublish(LogEngine.QueueAbstractMessage.ToArray().ToList());
                }

                if (LogEngine.QueueConsoleMessage != null)
                {
                    LogEngine.QueueConsoleMessageOnPublish(LogEngine.QueueConsoleMessage.ToArray().ToList());
                }
            }
        }

        public static void StartEngine()
        {
            try
            {
                ConfigurationBag.LoadConfiguration();
                LogEngine.Init();
                Trace.WriteLine(
                    $"Version {Assembly.GetExecutingAssembly().GetName().Version}");

                Trace.WriteLine("--SnapGate Sevice Initialization--Start Engine.");
                _delegate = delegateActionEventEmbedded;
                CoreEngine.StartEventEngine(_delegate);
                engineLoaded = true;
                Thread.Sleep(Timeout.Infinite);
            }
            catch (NotImplementedException ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    "Error in " + MethodBase.GetCurrentMethod().Name,
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    "Error in " + MethodBase.GetCurrentMethod().Name,
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
                Environment.Exit(0);
            } // try/catch
            finally
            {
                //Spool log queues
                if (LogEngine.QueueAbstractMessage != null)
                {
                    LogEngine.QueueAbstractMessageOnPublish(LogEngine.QueueAbstractMessage.ToArray().ToList());
                }

                if (LogEngine.QueueConsoleMessage != null)
                {
                    LogEngine.QueueConsoleMessageOnPublish(LogEngine.QueueConsoleMessage.ToArray().ToList());
                }
            }
        }

        public static void EngineStartedAsync()
        {
            while (!engineLoaded) ;
        }

        /// <summary>
        ///     The delegate event executed by a event
        /// </summary>
        /// <param name="eventType">
        /// </param>
        /// <param name="context">
        ///     EventActionContext cosa deve essere restituito
        /// </param>
        private static void delegateActionEventEmbedded(IEventType eventType, ActionContext context)
        {
            try
            {
                //If embedded mode and trigger source == embeddedtrigger then not execute the internal embedded delelegate 
                //todo optimization qui controllavo se chi ha chiamato levento e un trigger? forse meglio usare un approccio diverso, ho rimosso il check fdel trigger, sembra inutile

                //   if (context.BubblingObjectBag.AssemblyClassType != typeof(SnapGate.Framework.EmbeddedTrigger.EmbeddedTrigger))
                setEventActionEventEmbedded(eventType, context);
            }
            catch (Exception ex)
            {
                context.BubblingObjectBag.CorrelationOverride = null;

                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
            }
        }

        /// <summary>
        ///     Load the bubbling settings
        /// </summary>
        public static void InitializeOffRampEmbedded(ActionEvent delegateEmbedded)
        {
            //Load Configuration
            ConfigurationBag.LoadConfiguration();

            LogEngine.WriteLog(ConfigurationBag.EngineName,
                "Inizialize Off Ramp embedded messaging.",
                Constant.LogLevelError,
                Constant.TaskCategoriesError,
                null,
                Constant.LogLevelInformation);

            //Solve App domain environment
            var current = AppDomain.CurrentDomain;
            current.AssemblyResolve += HandleAssemblyResolve;


            int triggers = 0;
            int events = 0;
            int components = 0;

            EventsEngine.InitializeTriggerEngine();
            EventsEngine.InitializeEmbeddedEvent(delegateEmbedded);
            //Load component list configuration
            EventsEngine.LoadAssemblyComponents(ref triggers, ref events, ref components);

            //Load event list configuration
            EventsEngine.RefreshBubblingSetting(false);
        }


        private static Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return CoreEngine.HandleAssemblyResolve(sender, args);
        }

        /// <summary>
        ///     Execute an internal trigger, this is used to execute a configured trigger
        ///     To use: configure a trigger and call it by
        /// </summary>
        /// <param name="componeId">
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        /// http://localhost:8000/SnapGate/ExecuteTrigger?TriggerID={3C62B951-C353-4899-8670-C6687B6EAEFC}
        public static bool ExecuteTrigger(string configurationId, string componeId, byte[] data)
        {
            try
            {
                var triggerSingleInstance = (from trigger in EventsEngine.BubblingTriggerConfigurationsSingleInstance
                    where trigger.IdComponent == componeId && trigger.IdConfiguration == configurationId
                    select trigger).First();
                EventsEngine.ExecuteTriggerConfiguration(triggerSingleInstance, data);
                return true;
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name} - The trigger ID {componeId} does not exist.",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
                return false;
            }
        }


        /// <summary>
        ///     Initialize an embedded trigger
        /// </summary>
        /// <param name="componeId">
        /// </param>
        /// <param name="configurationId"></param>
        /// <param name="componentId"></param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static TriggerEmbeddedBag InitializeEmbeddedTrigger(string configurationId, string componentId)
        {
            try
            {
                return EventsEngine.InitializeEmbeddedTrigger(configurationId, componentId);
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

        /// <summary>
        ///     Execute an embedded trigger
        /// </summary>
        /// <param name="componeId">
        /// </param>
        /// <param name="triggerEmbeddedBag"></param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static byte[] ExecuteSyncronousEmbeddedTrigger(TriggerEmbeddedBag triggerEmbeddedBag)
        {
            try
            {
                eventStop = new AutoResetEvent(false);
                SyncAsyncEventAction = SyncAsyncActionReceived;
                triggerEmbeddedBag.ActionContext.BubblingObjectBag.SyncronousToken = Guid.NewGuid().ToString();
                EventsEngine.SyncAsyncEventsAddDelegate(
                    triggerEmbeddedBag.ActionContext.BubblingObjectBag.SyncronousToken,
                    SyncAsyncActionReceived);

                EventsEngine.EngineExecuteEmbeddedTrigger(triggerEmbeddedBag);

                eventStop.WaitOne();

                return _syncronousDataContext;
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

        public static byte[] ExecuteAsyncronousEmbeddedTrigger(TriggerEmbeddedBag triggerEmbeddedBag)
        {
            try
            {
                return EventsEngine.EngineExecuteEmbeddedTrigger(triggerEmbeddedBag);
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

        public static void SyncAsyncActionReceived(byte[] content)
        {
            _syncronousDataContext = content;
            eventStop.Set();
        }
    }
}