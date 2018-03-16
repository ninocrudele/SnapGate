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
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Engine.OffRamp;
using SnapGate.Framework.Engine.OnRamp;
using SnapGate.Framework.Log;
using SnapGate.Framework.PBI;

#endregion

namespace SnapGate.Framework.Engine
{
    /// <summary>
    ///     Primary engine, it start all, this is the first point
    /// </summary>
    public static class CoreEngine
    {
        private static int pollingStep;
        public static PBIEngineProvider pBIEngineProvider { get; set; }

        public static Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
        {
            /* Load the assembly specified in 'args' here and return it, 
               if the assembly is already loaded you can return it here */
            try
            {
                if (args.Name.Substring(0, 10) == "Microsoft.")
                {
                    return null;
                }

                if (args.Name.Substring(0, 7) == "System.")
                {
                    return null;
                }

                return EventsEngine.CacheEngineComponents[args.Name];
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(
                    $"Critical error in {MethodBase.GetCurrentMethod().Name} - The Assembly [{args.Name}] not found.");
                sb.AppendLine(
                    "Workaround: this error because a trigger or event is looking for a particular external library in reference, check if all the libraries referenced by triggers and events are in the triggers and events directories dll or registered in GAC.");

                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    sb.ToString(),
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
                return null;
            }
        }

        /// <summary>
        ///     Execute polling
        /// </summary>
        /// restart
        public static void StartEventEngine(ActionEvent delegateEmbedded)
        {
            try
            {
                LogEngine.DirectEventViewerLog("Engine starting...", 4);
                var current = AppDomain.CurrentDomain;
                current.AssemblyResolve += HandleAssemblyResolve;

                LogEngine.Enabled = ConfigurationBag.Configuration.LoggingEngineEnabled;
                Trace.WriteLine("Load Engine configuration.");

                if (ConfigurationBag.Configuration.CleanPowerBIDataset)
                {
                    LogEngine.DirectEventViewerLog(
                        "CleanPowerBIDataset set to true, the Power BI engine dataset will be cleaned.", 2);
                }

                //****************************Check for updates
                //Check if need to update files received from partners
                Trace.WriteLine(
                    $"Check Engine Syncronization {ConfigurationBag.Configuration.AutoSyncronizationEnabled}.");
                if (ConfigurationBag.Configuration.AutoSyncronizationEnabled)
                {
                    EventsEngine.SyncronizePoint();
                }


                //Set service states
                Trace.WriteLine("Initialize Engine Service states.");
                ServiceStates.RunPolling = ConfigurationBag.Configuration.RunInternalPolling;
                ServiceStates.RestartNeeded = false;

                Trace.WriteLine("Initialize Engine.");
                EventsEngine.InitializeEventEngine(delegateEmbedded);

                //Init Message ingestor
                MessageIngestor.InitSecondaryPersistProvider();

                //Create the two sends layers
                Trace.WriteLine("Start Internal Event Engine Channel.");


                //in EventUpStream
                Trace.WriteLine("Start External Event Engine Channel.");
                //OffRamp start the OnRamp Engine
                var canStart = OffRampEngineSending.Init("MSP Device Component.dll (vNext)");

                if (!canStart)
                {
                    LogEngine.WriteLog(
                        ConfigurationBag.EngineName,
                        $"Error during engine service starting. Name: {ConfigurationBag.Configuration.ChannelName} - ID: {ConfigurationBag.Configuration.ChannelId}",
                        Constant.LogLevelError,
                        Constant.TaskCategoriesError,
                        null,
                        Constant.LogLevelError);
                    Thread.Sleep(ConfigurationBag.Configuration.WaitTimeBeforeRestarting);
                    Environment.Exit(0);
                }

                if (ConfigurationBag.Configuration.RunLocalOny)
                {
                    LogEngine.DirectEventViewerLog("The RunLocalOny is true, the engine will run in local mode only.",
                        2);
                }


                //Start PowerBI engine if enabled
                //PowerBI log engine
                if (ConfigurationBag.Configuration.PowerBIEnabled)
                {
                    //Load configuration
                    //var dataDomainConfiguration = new List<dynamic>();
                    //var stringPropertyNamesAndValues = ConfigurationBag.Configuration.GetType()
                    //    .GetProperties()
                    //    .Where(pi => pi.PropertyType == typeof(string) && pi.GetGetMethod() != null)
                    //    .Select(pi => new
                    //    {
                    //        Name = pi.Name,
                    //        Value = pi.GetGetMethod().Invoke(ConfigurationBag.Configuration, null)
                    //    });

                    //dynamic expandoObjectConfiguration = new ExpandoObject();
                    //foreach (var field in stringPropertyNamesAndValues)
                    //{

                    //    ((IDictionary<string, object>)expandoObjectConfiguration)[field.Name] = field.Value;
                    //}
                    //dataDomainConfiguration.Add(expandoObjectConfiguration);


                    LogEngine.DirectEventViewerLog("PowerBI engine features is enabled.", 4);
                    var dataDomain = new List<dynamic>();
                    dynamic expandoObject = new ExpandoObject();
                    //Configuration Area
                    expandoObject.RunLocalOny = ConfigurationBag.Configuration.RunLocalOny;
                    expandoObject.HAGroup = ConfigurationBag.Configuration.HAGroup;
                    expandoObject.HACheckTime = ConfigurationBag.Configuration.HACheckTime;
                    expandoObject.HAInactivity = ConfigurationBag.Configuration.HAInactivity;
                    expandoObject.AutoSyncronizationEnabled = ConfigurationBag.Configuration.AutoSyncronizationEnabled;
                    expandoObject.RunInternalPolling = ConfigurationBag.Configuration.RunInternalPolling;
                    expandoObject.EncodingType = ConfigurationBag.Configuration.EncodingType.ToString();
                    //expandoObject.AzureNameSpaceConnectionString =
                    //    ConfigurationBag.Configuration.AzureNameSpaceConnectionString;
                    //expandoObject.GroupStorageAccountKey = ConfigurationBag.Configuration.GroupStorageAccountKey;
                    expandoObject.GroupStorageAccountName = ConfigurationBag.Configuration.GroupStorageAccountName;
                    expandoObject.GroupEventHubsName = ConfigurationBag.Configuration.GroupEventHubsName;
                    expandoObject.EventHubsStartingDateTimeReceiving =
                        ConfigurationBag.Configuration.EventHubsStartingDateTimeReceiving;
                    expandoObject.EventHubsEpoch = ConfigurationBag.Configuration.EventHubsEpoch;
                    expandoObject.ServiceBusConnectivityMode =
                        ConfigurationBag.Configuration.ServiceBusConnectivityMode.ToString();
                    expandoObject.EventHubsCheckPointPattern =
                        ConfigurationBag.Configuration.EventHubsCheckPointPattern.ToString();
                    expandoObject.EventHubsReceivingPattern =
                        ConfigurationBag.Configuration.EventHubsReceivingPattern.ToString();
                    expandoObject.RedisConnectionString = ConfigurationBag.Configuration.RedisConnectionString;
                    expandoObject.EventsStreamComponent = ConfigurationBag.Configuration.EventsStreamComponent;
                    expandoObject.PersistentProviderComponent =
                        ConfigurationBag.Configuration.PersistentProviderComponent;
                    expandoObject.SecondaryPersistProviderEnabled =
                        ConfigurationBag.Configuration.SecondaryPersistProviderEnabled;
                    expandoObject.SecondaryPersistProviderByteSize =
                        ConfigurationBag.Configuration.SecondaryPersistProviderByteSize;
                    expandoObject.EnablePersistingMessaging = ConfigurationBag.Configuration.EnablePersistingMessaging;
                    expandoObject.Clustered = ConfigurationBag.Configuration.Clustered;
                    expandoObject.ClusterBaseFolder = ConfigurationBag.Configuration.ClusterBaseFolder;
                    expandoObject.WaitTimeBeforeRestarting = ConfigurationBag.Configuration.WaitTimeBeforeRestarting;
                    expandoObject.WebApiEndPoint = ConfigurationBag.Configuration.WebApiEndPoint;
                    expandoObject.PointID = ConfigurationBag.Configuration.PointId;
                    expandoObject.PointName = ConfigurationBag.Configuration.PointName;
                    expandoObject.PointDescription = ConfigurationBag.Configuration.PointDescription;
                    expandoObject.ChannelID = ConfigurationBag.Configuration.ChannelId;
                    expandoObject.ChannelName = ConfigurationBag.Configuration.ChannelName;
                    expandoObject.ChannelDescription = ConfigurationBag.Configuration.ChannelDescription;
                    expandoObject.LocalStorageConnectionString =
                        ConfigurationBag.Configuration.LocalStorageConnectionString;
                    expandoObject.LocalStorageProvider = ConfigurationBag.Configuration.LocalStorageProvider;
                    expandoObject.LoggingLevel = ConfigurationBag.Configuration.LoggingLevel;
                    expandoObject.LoggingEngineEnabled = ConfigurationBag.Configuration.LoggingEngineEnabled;
                    expandoObject.PowerBIEnabled = ConfigurationBag.Configuration.PowerBIEnabled;
                    expandoObject.EnginePollingTime = ConfigurationBag.Configuration.EnginePollingTime;
                    expandoObject.LoggingComponent = ConfigurationBag.Configuration.LoggingComponent;
                    expandoObject.LoggingComponentStorage = ConfigurationBag.Configuration.LoggingComponentStorage;
                    expandoObject.ThrottlingOnRampIncomingRateNumber =
                        ConfigurationBag.Configuration.ThrottlingOnRampIncomingRateNumber;
                    expandoObject.ThrottlingOnRampIncomingRateSeconds =
                        ConfigurationBag.Configuration.ThrottlingOnRampIncomingRateSeconds;
                    expandoObject.ThrottlingOffRampIncomingRateNumber =
                        ConfigurationBag.Configuration.ThrottlingOffRampIncomingRateNumber;
                    expandoObject.ThrottlingOffRampIncomingRateSeconds =
                        ConfigurationBag.Configuration.ThrottlingOffRampIncomingRateSeconds;
                    expandoObject.ThrottlingConsoleLogIncomingRateNumber = ConfigurationBag.Configuration
                        .ThrottlingConsoleLogIncomingRateNumber;
                    expandoObject.ThrottlingConsoleLogIncomingRateSeconds = ConfigurationBag.Configuration
                        .ThrottlingConsoleLogIncomingRateSeconds;
                    expandoObject.ThrottlingLSILogIncomingRateNumber =
                        ConfigurationBag.Configuration.ThrottlingLsiLogIncomingRateNumber;
                    expandoObject.ThrottlingLSILogIncomingRateSeconds =
                        ConfigurationBag.Configuration.ThrottlingLsiLogIncomingRateSeconds;
                    expandoObject.MaxWorkerThreads = ConfigurationBag.Configuration.MaxWorkerThreads;
                    expandoObject.MaxAsyncWorkerThreads = ConfigurationBag.Configuration.MaxAsyncWorkerThreads;
                    expandoObject.MinWorkerThreads = ConfigurationBag.Configuration.MinWorkerThreads;
                    expandoObject.MinAsyncWorkerThreads = ConfigurationBag.Configuration.MinAsyncWorkerThreads;
                    //Event Area

                    expandoObject.MessageID = "";
                    expandoObject.SenderPointId = "";
                    expandoObject.SenderName = "";
                    expandoObject.SenderDescriprion = "";
                    expandoObject.SenderChannelId = "";
                    expandoObject.SenderChannelName = "";
                    expandoObject.SenderChannelDescription = "";

                    expandoObject.TargetPointID = ConfigurationBag.Configuration.PointId;
                    expandoObject.TargetPointName = ConfigurationBag.Configuration.PointName;
                    expandoObject.TargetPointDescription = ConfigurationBag.Configuration.PointDescription;
                    expandoObject.TargetChannelID = ConfigurationBag.Configuration.ChannelId;
                    expandoObject.TargetChannelName = ConfigurationBag.Configuration.ChannelName;
                    expandoObject.TargetChannelDescription = ConfigurationBag.Configuration.ChannelDescription;

                    expandoObject.MessageSize = 0;
                    expandoObject.MessageType = "";
                    expandoObject.BubblingEventType = "";
                    expandoObject.Chains = 0;
                    expandoObject.Description = "";
                    expandoObject.Event = "";
                    expandoObject.Name = "";
                    expandoObject.Trigger = "";
                    expandoObject.Messages = 1;
                    expandoObject.ErrorCode = "";
                    expandoObject.ErrorDescription = "";

                    //string a = JsonConvert.SerializeObject(
                    //    expandoObject,
                    //    Formatting.Indented,
                    //    new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    //File.WriteAllText("c:\\Configuration.json", a);

                    dataDomain.Add(expandoObject);
                    pBIEngineProvider = new PBIEngineProvider();
                    pBIEngineProvider.CreatePBIProvider(dataDomain, "IntegrationPoints", "IntPoints");
                    //PBIEngineProvider.CreatePBIProvider(dataDomain, "ConfigurationPoints", "ConfPoints");
                }
                else
                {
                    LogEngine.DirectEventViewerLog("PowerBI engine features is not enabled.", 4);
                }

                if (EventsEngine.HAEnabled)
                {
                    Thread haCheck = new Thread(EventsEngine.HAPointsUpdate);
                    haCheck.Start();
                    Thread haClean = new Thread(EventsEngine.HAPointsClean);
                    haClean.Start();
                }


                //*****************Event object stream area*********************
                //Load the global event and triggers dlls
                var numOfTriggers = 0;
                var numOfEvents = 0;
                var numOfComponents = 0;

                var triggersAndEventsLoaded = EventsEngine.LoadAssemblyComponents(ref numOfTriggers, ref numOfEvents,
                    ref numOfComponents);
                if (triggersAndEventsLoaded)
                {
                    Trace.WriteLine(
                        $"Triggers loaded {numOfTriggers} - Events loaded {numOfEvents}");
                }

                //Load the Active triggers and the active events
                EventsEngine.RefreshBubblingSetting(false);
                //Start triggers single instances
                EventsEngine.ExecuteBubblingTriggerConfigurationsSingleInstance();
                //Start triggers polling instances
                if (ConfigurationBag.Configuration.EnginePollingTime > 0)
                {
                    var treadPollingRun = new Thread(StartTriggerPolling);
                    treadPollingRun.Start();
                }
                else
                {
                    LogEngine.WriteLog(ConfigurationBag.EngineName,
                        $"Configuration.EnginePollingTime = {ConfigurationBag.Configuration.EnginePollingTime}, internal polling system disabled.",
                        Constant.LogLevelError,
                        Constant.TaskCategoriesError,
                        null,
                        Constant.LogLevelWarning);
                }

                //Start Engine Service
                Trace.WriteLine("Asyncronous Threading Service state active.");
                var treadEngineStates = new Thread(CheckServiceStates);
                treadEngineStates.Start();

                Trace.WriteLine("Start On Ramp Engine.");

                if (ConfigurationBag.Configuration.RunLocalOny)
                {
                    LogEngine.WriteLog(ConfigurationBag.EngineName,
                        $"Abstract Event Up Stream Engine not defined, the on ramp engine (receiving messages) will work in local mode only.",
                        Constant.LogLevelInformation,
                        Constant.TaskCategoriesError,
                        null,
                        Constant.LogLevelInformation);
                    return;
                }

                var onRampEngineReceiving = new OnRampEngineReceiving();
                onRampEngineReceiving.Init(string.Empty);

                // Configuration files watcher
                //****************************Start folder whatcher
                EventsEngine.StartFolderWatcherEngine();
                //****************************Start folder whatcher


                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Engine service initialization procedure terminated. Name: {ConfigurationBag.Configuration.ChannelName} - ID: {ConfigurationBag.Configuration.ChannelId}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    null,
                    Constant.LogLevelInformation);
                LogEngine.DirectEventViewerLog("Engine started.", 4);
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
            }
        }

        public static void StopEventEngine()
        {
            //EventsEngine.DisposeEngine();
        }

        /// <summary>
        ///     If restart required it perform the operations
        /// </summary>
        public static void CheckServiceStates()
        {
            while (true)
            {
                Thread.Sleep(10000);
                if (ServiceStates.RestartNeeded)
                {
                    Trace.WriteLine("--------------------------------------------------------");
                    Trace.WriteLine("Service needs restarting.");
                    Trace.WriteLine("--------------------------------------------------------");
                    ServiceStates.RestartNeeded = false;
                    //Thread.Sleep(ConfigurationBag.Configuration.WaitTimeBeforeRestarting);
                }
            }

            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        ///     Execute polling
        /// </summary>
        private static void StartTriggerPolling()
        {
            //running thread polling
            var pollingTime = ConfigurationBag.Configuration.EnginePollingTime;
            try
            {
                if (pollingTime == 0)
                {
                    LogEngine.WriteLog(
                        ConfigurationBag.EngineName,
                        $"EnginePollingTime = 0 - Internal logging system disabled.",
                        Constant.LogLevelError,
                        Constant.TaskCategoriesError,
                        null,
                        Constant.LogLevelWarning);
                    return;
                }

                Trace.WriteLine("Start Trigger Polling Cycle");


                while (ServiceStates.RunPolling)
                {
                    if (ServiceStates.RestartNeeded)
                    {
                        Trace.WriteLine("--------------------------------------------------------");
                        Trace.WriteLine("- UPDATE READY - SERVICE NEEDS RESTART -");
                        Trace.WriteLine("--------------------------------------------------------");
                        //ServiceStates.RunPolling = false;
                        return;
                    }

                    ++pollingStep;
                    if (pollingStep > 9)
                    {
                        Trace.WriteLine($"Execute Trigger Polling {pollingStep} Cycle");
                        pollingStep = 0;
                    }

                    Thread.Sleep(pollingTime);
                    var treadPollingRun = new Thread(EventsEngine.ExecuteBubblingTriggerConfigurationPolling);
                    treadPollingRun.Start();
                }
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
            }
        }
    }
}