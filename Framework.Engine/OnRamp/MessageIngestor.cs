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
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using SnapGate.Framework.Base;
using SnapGate.Framework.CompressionLibrary;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Storage;
using SnapGate.Framework.Engine.OffRamp;
using SnapGate.Framework.Log;
using SnapGate.Framework.Serialization.Object;

#endregion

namespace SnapGate.Framework.Engine.OnRamp
{
    /// <summary>
    ///     Engine main message ingestor
    /// </summary>
    public static class MessageIngestor
    {
        public delegate void SetConsoleActionEventEmbedded(
            string DestinationConsolePointId, IBubblingObject bubblingObject);


        private static bool secondaryPersistProviderEnabled;
        private static int secondaryPersistProviderByteSize;
        private static IDevicePersistentProvider DevicePersistentProvider;
        private static readonly object[] ParametersPersistEventFromBlob = {null};


        /// <summary>
        ///     Used internally by the embedded
        /// </summary>
        public static SetConsoleActionEventEmbedded setConsoleActionEventEmbedded { get; set; }

        public static void InitSecondaryPersistProvider()
        {
            try
            {
                secondaryPersistProviderEnabled = ConfigurationBag.Configuration.SecondaryPersistProviderEnabled;
                secondaryPersistProviderByteSize = ConfigurationBag.Configuration.SecondaryPersistProviderByteSize;

                // Load the abrstracte persistent provider
                var devicePersistentProviderComponent = Path.Combine(
                    ConfigurationBag.Configuration.DirectoryOperativeRootExeName,
                    ConfigurationBag.Configuration.PersistentProviderComponent);

                // Create the reflection method cached 
                var assemblyPersist = Assembly.LoadFrom(devicePersistentProviderComponent);

                // Main class logging
                var assemblyClassDpp = (from t in assemblyPersist.GetTypes()
                    let attributes = t.GetCustomAttributes(typeof(DevicePersistentProviderContract), true)
                    where t.IsClass && attributes != null && attributes.Length > 0
                    select t).First();


                DevicePersistentProvider = Activator.CreateInstance(assemblyClassDpp) as IDevicePersistentProvider;
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesEventHubs,
                    ex,
                    Constant.LogLevelError);
            }
        }

        public static void IngestMessagge(BubblingObject bubblingObject)
        {
            try
            {
                LogEngine.WriteLog(ConfigurationBag.EngineName,
                    $"IngestMessagge bubblingObject.SenderChannelId {bubblingObject.SenderChannelId} " +
                    $"bubblingObject.SenderPointId {bubblingObject.SenderPointId} " +
                    $"bubblingObject.DestinationChannelId {bubblingObject.DestinationChannelId} " +
                    $" bubblingObject.DestinationPointId {bubblingObject.DestinationPointId} " +
                    $"bubblingObject.MessageType {bubblingObject.MessageType}" +
                    $"bubblingObject.Persisting {bubblingObject.Persisting} " +
                    $"bubblingObject.MessageId {bubblingObject.MessageId} " +
                    $"bubblingObject.Name {bubblingObject.Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesConsole,
                    null,
                    Constant.LogLevelVerbose);

                //If local event then execute
                if (bubblingObject.LocalEvent)
                {
                    //new Task(() =>
                    //{
                    //    EventsEngine.ExecuteEventsInTrigger(
                    //    bubblingObject,
                    //    bubblingObject.Events[0],
                    //    false,
                    //    bubblingObject.SenderPointId);

                    //}).Start();
                    EventsEngine.ExecuteEventsInTrigger(
                        bubblingObject,
                        bubblingObject.Events[0],
                        false,
                        bubblingObject.SenderPointId);
                    return;
                }

                //Check if message is for this point
                var receiverChannelId = bubblingObject.DestinationChannelId;
                var receiverPointId = bubblingObject.DestinationPointId;

                var requestAvailable = (receiverChannelId == ConfigurationBag.Configuration.ChannelId
                                        && receiverPointId == ConfigurationBag.Configuration.PointId)
                                       || (receiverChannelId == ConfigurationBag.ChannelAll
                                           && receiverPointId == ConfigurationBag.Configuration.PointId)
                                       || (receiverChannelId == ConfigurationBag.Configuration.ChannelId
                                           && receiverPointId == ConfigurationBag.PointAll)
                                       || (receiverChannelId == ConfigurationBag.ChannelAll
                                           && receiverPointId == ConfigurationBag.PointAll);

                if (!requestAvailable)
                {
                    // ****************************NOT FOR ME*************************
                    return;
                }

                if (bubblingObject.SenderPointId == ConfigurationBag.Configuration.PointId)
                {
                    // **************************** HA AREA *************************


                    if (bubblingObject.MessageType == "HA" &&
                        bubblingObject.HAGroup == ConfigurationBag.Configuration.HAGroup &&
                        EventsEngine.HAEnabled)
                    {
                        //If HA group member and HA 

                        EventsEngine.HAPoints[EventsEngine.HAPointTickId] = DateTime.Now;
                        long haTickFrom = long.Parse(UTF8Encoding.UTF8.GetString(bubblingObject.Data));

                        //if same tick then return because same point
                        if (haTickFrom == EventsEngine.HAPointTickId)
                            return;

                        DateTime dt;
                        lock (EventsEngine.HAPoints)
                        {
                            //If not exist then add
                            if (!EventsEngine.HAPoints.TryGetValue(haTickFrom, out dt))
                                EventsEngine.HAPoints.Add(haTickFrom, DateTime.Now);
                            else
                            {
                                EventsEngine.HAPoints[haTickFrom] = DateTime.Now;
                            }
                        }

                        byte[] content = UTF8Encoding.UTF8.GetBytes(EventsEngine.HAPointTickId.ToString());

                        BubblingObject bubblingObjectToSync = new BubblingObject(content);

                        bubblingObject.MessageType = "HA";
                        OffRampEngineSending.SendMessageOffRamp(bubblingObjectToSync,
                            "HA",
                            bubblingObject.SenderChannelId,
                            bubblingObject.SenderPointId,
                            string.Empty);
                    }
                    else
                    {
                        return;
                    }
                }

                // ****************************GET FROM STORAGE IF REQUIRED*************************
                if (bubblingObject.Persisting)
                {
                    bubblingObject =
                        (BubblingObject)
                        SerializationEngine.ByteArrayToObject(
                            DevicePersistentProvider.PersistEventFromStorage(bubblingObject.MessageId));
                }

                #region EVENT

                // ****************************IF EVENT TYPE*************************
                if (bubblingObject.MessageType == "Event")
                {
                    //PowerBI log engine
                    if (ConfigurationBag.Configuration.PowerBIEnabled)
                    {
                        var dataDomain = new List<dynamic>();
                        dynamic expandoObject = new ExpandoObject();
                        //Configuration Area
                        expandoObject.RunLocalOny = ConfigurationBag.Configuration.RunLocalOny;
                        expandoObject.HAGroup = ConfigurationBag.Configuration.HAGroup;
                        expandoObject.HACheckTime = ConfigurationBag.Configuration.HACheckTime;
                        expandoObject.HAInactivity = ConfigurationBag.Configuration.HAInactivity;
                        expandoObject.AutoSyncronizationEnabled =
                            ConfigurationBag.Configuration.AutoSyncronizationEnabled;
                        expandoObject.RunInternalPolling = ConfigurationBag.Configuration.RunInternalPolling;
                        expandoObject.EncodingType = ConfigurationBag.Configuration.EncodingType.ToString();
                        //expandoObject.AzureNameSpaceConnectionString = ConfigurationBag.Configuration.AzureNameSpaceConnectionString;
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
                        expandoObject.EnablePersistingMessaging =
                            ConfigurationBag.Configuration.EnablePersistingMessaging;
                        expandoObject.Clustered = ConfigurationBag.Configuration.Clustered;
                        expandoObject.ClusterBaseFolder = ConfigurationBag.Configuration.ClusterBaseFolder;
                        expandoObject.WaitTimeBeforeRestarting =
                            ConfigurationBag.Configuration.WaitTimeBeforeRestarting;
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
                        expandoObject.ThrottlingConsoleLogIncomingRateNumber =
                            ConfigurationBag.Configuration.ThrottlingConsoleLogIncomingRateNumber;
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

                        expandoObject.MessageID = bubblingObject.MessageId;
                        expandoObject.SenderPointId = bubblingObject.SenderPointId;
                        expandoObject.SenderName = bubblingObject.SenderName;
                        expandoObject.SenderDescriprion = bubblingObject.SenderDescriprion;
                        expandoObject.SenderChannelId = bubblingObject.SenderChannelId;
                        expandoObject.SenderChannelName = bubblingObject.SenderChannelName;
                        expandoObject.SenderChannelDescription = bubblingObject.SenderChannelDescription;

                        expandoObject.TargetPointID = ConfigurationBag.Configuration.PointId;
                        expandoObject.TargetPointName = ConfigurationBag.Configuration.PointName;
                        expandoObject.TargetPointDescription = ConfigurationBag.Configuration.PointDescription;
                        expandoObject.TargetChannelID = ConfigurationBag.Configuration.ChannelId;
                        expandoObject.TargetChannelName = ConfigurationBag.Configuration.ChannelName;
                        expandoObject.TargetChannelDescription = ConfigurationBag.Configuration.ChannelDescription;

                        expandoObject.MessageSize = bubblingObject.DataContext.Length;
                        expandoObject.MessageType = bubblingObject.MessageType;
                        expandoObject.BubblingEventType = bubblingObject.BubblingEventType;
                        expandoObject.Chains = bubblingObject.Chains?.Count ?? 0;
                        expandoObject.Description = bubblingObject.Description;
                        expandoObject.Event = bubblingObject.Event;
                        expandoObject.Name = bubblingObject.Name;
                        string fakeValue = String.Empty;
                        expandoObject.ErrorCode = fakeValue;
                        expandoObject.ErrorDescription = fakeValue;

                        expandoObject.Trigger = bubblingObject.Trigger;


                        expandoObject.Messages = 1;

                        dataDomain.Add(expandoObject);
                        CoreEngine.pBIEngineProvider.SendData(dataDomain);

                        //var requestAvailable = (receiverChannelId == ConfigurationBag.Configuration.ChannelId
                        //                        && receiverPointId == ConfigurationBag.Configuration.PointId)
                        //                       || (receiverChannelId == ConfigurationBag.ChannelAll
                        //                           && receiverPointId == ConfigurationBag.Configuration.PointId)
                        //                       || (receiverChannelId == ConfigurationBag.Configuration.ChannelId
                        //                           && receiverPointId == ConfigurationBag.PointAll)
                        //                       || (receiverChannelId == ConfigurationBag.ChannelAll
                        //                           && receiverPointId == ConfigurationBag.PointAll);
                    }

                    //If HA group member and HA 
                    if (EventsEngine.HAEnabled)
                    {
                        //Check if is the first in list, if not then discard
                        EventsEngine.HAPoints.OrderBy(key => key.Key);
                        if (EventsEngine.HAPoints.Count > 1 &&
                            EventsEngine.HAPoints.First().Key != EventsEngine.HAPointTickId)
                        {
                            return;
                        }
                    }

                    //Check if is Syncronouse response
                    if (bubblingObject.SyncronousFromEvent &&
                        bubblingObject.SenderPointId == ConfigurationBag.Configuration.PointId)
                    {
                        //Yes it's a syncronous response from my request from this pointid
                        //Execute the delegate and exit
                        var propDataContext = bubblingObject.DataContext;
                        bubblingObject.SyncronousFromEvent = false;
                        EventsEngine.SyncAsyncEventsExecuteDelegate(bubblingObject.SyncronousToken, propDataContext);
                        bubblingObject.SenderPointId = "";
                        bubblingObject.SyncronousToken = "";
                        return;
                    }

                    // ****************************EVENT EXIST EXECUTE*************************
                    EventsEngine.ExecuteEventsInTrigger(
                        bubblingObject,
                        bubblingObject.Events[0],
                        false,
                        bubblingObject.SenderPointId);
                    return;
                }

                #endregion

                #region SYNC

                // **************************** SYNC AREA *************************

                //******************* OPERATION CONF BAG- ALL THE CONF FILES AND DLLS ****************************************************************
                //Receive a package folder to syncronize him self
                if (bubblingObject.MessageType == "SyncPull")
                {
                    byte[] content =
                        Helpers.CreateFromDirectory(
                            ConfigurationBag.Configuration.DirectoryOperativeRootExeName);

                    BubblingObject bubblingObjectToSync = new BubblingObject(content);
                    bubblingObject.MessageType = "SyncPush";
                    OffRampEngineSending.SendMessageOffRamp(bubblingObjectToSync,
                        "SyncPush",
                        bubblingObject.SenderChannelId,
                        bubblingObject.SenderPointId,
                        string.Empty);
                }

                //Receive the request to send the bubbling
                if (bubblingObject.MessageType == "SyncPush")
                {
                    LogEngine.DirectEventViewerLog(
                        $"Received a syncronization package from channel ID {bubblingObject.SenderChannelId} and point ID {bubblingObject.SenderChannelId}\rAutoSyncronizationEnabled parameter = {ConfigurationBag.Configuration.AutoSyncronizationEnabled}",
                        2);
                    if (ConfigurationBag.Configuration.AutoSyncronizationEnabled)
                    {
                        byte[] bubblingContent = SerializationEngine.ObjectToByteArray(bubblingObject.Data);
                        string currentSyncFolder = ConfigurationBag.SyncDirectorySyncIn();
                        Helpers.CreateFromBytearray(bubblingObject.Data, currentSyncFolder);

                        //Restart Application
                        if (ConfigurationBag.Configuration.WaitTimeBeforeRestarting > 0)
                        {
                            LogEngine.DirectEventViewerLog(
                                $"Syncronization event receive, WaitTimeBeforeRestarting > 0, the service will restart in {ConfigurationBag.Configuration.WaitTimeBeforeRestarting / 1000} seconds.\rSet WaitTimeBeforeRestarting = 0 to disable the auto restart.",
                                2);
                            Thread.Sleep(ConfigurationBag.Configuration.WaitTimeBeforeRestarting);
                            Environment.Exit(-1);
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
            }
        }
    }
}