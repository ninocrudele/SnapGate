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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Messaging;
using SnapGate.Framework.Contracts.Storage;
using SnapGate.Framework.Engine.OnRamp;
using SnapGate.Framework.Log;
using SnapGate.Framework.Serialization.Object;

#endregion

namespace SnapGate.Framework.Engine.OffRamp
{
    /// <summary>
    ///     Internal messaging Queue
    /// </summary>
    public sealed class OffRampEngineQueue : LockSlimQueueEngine<BubblingObject>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OffRampEngineQueue" /> class.
        /// </summary>
        /// <param name="capLimit">
        ///     TODO The cap limit.
        /// </param>
        /// <param name="timeLimit">
        ///     TODO The time limit.
        /// </param>
        public OffRampEngineQueue(int capLimit, int timeLimit)
        {
            CapLimit = capLimit;
            TimeLimit = timeLimit;
            InitTimer();
        }
    }

    /// <summary>
    ///     Last line of receiving and first before the message ingestor
    /// </summary>
    public static class OffRampEngineSending
    {
        //Performance counter

        /// <summary>
        ///     The off ramp engine.
        /// </summary>
        private static OffRampEngineQueue OffRampEngineQueue;

        /// <summary>
        ///     Off Ramp Component
        /// </summary>
        private static IOffRampStream OffRampStream;

        private static IDevicePersistentProvider DevicePersistentProvider;

        private static bool secondaryPersistProviderEnabled;

        private static int secondaryPersistProviderByteSize;

        /// <summary>
        ///     Initialize the onramp engine the OffRampPatternComponent variable is for the next version
        /// </summary>
        /// <param name="offRampPatternComponent">
        ///     The Off Ramp Pattern Component.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool Init(string offRampPatternComponent)
        {
            try
            {
                LogEngine.WriteLog(ConfigurationBag.EngineName,
                    $"Start engine initialization.",
                    Constant.LogLevelInformation,
                    Constant.TaskCategoriesError,
                    null,
                    Constant.LogLevelInformation);

                Trace.WriteLine("Initialize Abstract Event Up Stream Engine.");

                // Load event up stream external component
                var eventsUpStreamComponent = Path.Combine(
                    ConfigurationBag.Configuration.DirectoryOperativeRootExeName,
                    ConfigurationBag.Configuration.EventsStreamComponent);

                // Create the reflection method cached 
                var assembly = Assembly.LoadFrom(eventsUpStreamComponent);

                // Main class logging
                var assemblyClass = (from t in assembly.GetTypes()
                    let attributes = t.GetCustomAttributes(typeof(EventsOffRampContract), true)
                    where t.IsClass && attributes != null && attributes.Length > 0
                    select t).First();

                OffRampStream = Activator.CreateInstance(assemblyClass) as IOffRampStream;

                OffRampEngineQueue = new OffRampEngineQueue(
                    ConfigurationBag.Configuration.ThrottlingOffRampIncomingRateNumber,
                    ConfigurationBag.Configuration.ThrottlingOffRampIncomingRateSeconds);
                OffRampEngineQueue.OnPublish += OffRampEngineQueueOnPublish;

                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    "Start Off Ramp Engine.",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    null,
                    Constant.LogLevelInformation);

                // Inizialize the Dpp
                Trace.WriteLine("Initialize Abstract Storage Provider Engine.");
                //todo optimization utilizzare linterfaccia
                OffRampStream.CreateOffRampStream();
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

                return true;
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
                return false;
            }
        }

        /// <summary>
        ///     Queue the message directly into the spool queue
        /// </summary>
        /// <param name="bubblingObject"></param>
        public static void QueueMessage(BubblingObject bubblingObject)
        {
            OffRampEngineQueue.Enqueue(bubblingObject);
        }

        /// <summary>
        ///     TODO The send message on ramp.
        /// </summary>
        /// <param name="bubblingTriggerConfiguration">
        ///     TODO The bubbling trigger configuration.
        /// </param>
        /// <param name="ehMessageType">
        ///     TODO The eh message type.
        /// </param>
        /// <param name="channelId">
        ///     TODO The channel id.
        /// </param>
        /// <param name="pointId">
        ///     TODO The point id.
        /// </param>
        /// <param name="properties">
        ///     TODO The properties.
        /// </param>
        public static void SendMessageOffRamp(
            BubblingObject bubblingObject,
            string messageType,
            string channelId,
            string pointId,
            string pointIdOverrided)
        {
            try
            {
                //Create message Id
                bubblingObject.MessageId = Guid.NewGuid().ToString();

                //Create bubbling object message to send
                byte[] serializedMessage = SerializationEngine.ObjectToByteArray(bubblingObject);

                //If enabled and > secondaryPersistProviderByteSize then store the body
                //put the messageid in the body message to recover
                bubblingObject.Persisting = serializedMessage.Length > secondaryPersistProviderByteSize &&
                                            secondaryPersistProviderEnabled;

                if (bubblingObject.Persisting)
                {
                    bubblingObject.Data = EncodingDecoding.EncodingString2Bytes(bubblingObject.MessageId);
                    DevicePersistentProvider.PersistEventToStorage(serializedMessage, bubblingObject.MessageId);
                    bubblingObject.Persisting = true;
                }

                // Message context
                bubblingObject.MessageType = messageType;

                if (pointIdOverrided != null)
                    bubblingObject.SenderPointId = pointIdOverrided;
                else
                    bubblingObject.SenderPointId = ConfigurationBag.Configuration.PointId;

                bubblingObject.SenderName = ConfigurationBag.Configuration.PointName;
                bubblingObject.SenderDescriprion = ConfigurationBag.Configuration.PointDescription;
                bubblingObject.SenderChannelId = ConfigurationBag.Configuration.ChannelId;
                bubblingObject.SenderChannelName = ConfigurationBag.Configuration.ChannelName;
                bubblingObject.SenderChannelDescription = ConfigurationBag.Configuration.ChannelDescription;
                bubblingObject.SenderPointId = ConfigurationBag.Configuration.PointId;
                bubblingObject.SenderName = ConfigurationBag.Configuration.PointName;
                bubblingObject.SenderDescriprion = ConfigurationBag.Configuration.PointDescription;
                bubblingObject.DestinationChannelId = channelId;
                bubblingObject.DestinationPointId = pointId;
                bubblingObject.HAGroup = ConfigurationBag.Configuration.HAGroup;

                LogEngine.WriteLog(ConfigurationBag.EngineName,
                    $"SendMessageOffRamp bubblingObject.SenderChannelId {bubblingObject.SenderChannelId} " +
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
                Trace.WriteLine($"Put message in OffRampQueue");

                //Call PowerBI Push
                //Parallel.Invoke(() => PushPowerBIData(bubblingObject));
                var task = Task.Run(() => PushPowerBIData(bubblingObject));


                OffRampEngineQueue.Enqueue(bubblingObject);
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

        public static void PushPowerBIData(BubblingObject bubblingObject)
        {
            //DEMODELETE
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
                expandoObject.AutoSyncronizationEnabled = ConfigurationBag.Configuration.AutoSyncronizationEnabled;
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
                expandoObject.PersistentProviderComponent = ConfigurationBag.Configuration.PersistentProviderComponent;
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
                expandoObject.ThrottlingConsoleLogIncomingRateNumber =
                    ConfigurationBag.Configuration.ThrottlingConsoleLogIncomingRateNumber;
                expandoObject.ThrottlingConsoleLogIncomingRateSeconds =
                    ConfigurationBag.Configuration.ThrottlingConsoleLogIncomingRateSeconds;
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
            }
        }

        public static void SendNullMessageOffRamp(
            string messageType,
            string channelId,
            string pointId,
            string idComponent,
            string subscriberId,
            string pointIdOverrided)
        {
            try
            {
                var bubblingObject = new BubblingObject(EncodingDecoding.EncodingString2Bytes(string.Empty));
                bubblingObject.Persisting = false;

                bubblingObject.MessageId = Guid.NewGuid().ToString();
                bubblingObject.SubscriberId = subscriberId;
                bubblingObject.MessageType = messageType;

                string senderid;

                if (pointIdOverrided != null)
                    senderid = pointIdOverrided;
                else
                    senderid = ConfigurationBag.Configuration.PointId;

                bubblingObject.SenderPointId = senderid;
                bubblingObject.SenderName = ConfigurationBag.Configuration.PointName;
                bubblingObject.SenderDescriprion = ConfigurationBag.Configuration.PointDescription;
                bubblingObject.SenderChannelId = ConfigurationBag.Configuration.ChannelId;
                bubblingObject.SenderChannelName = ConfigurationBag.Configuration.ChannelName;
                bubblingObject.SenderChannelDescription = ConfigurationBag.Configuration.ChannelDescription;
                bubblingObject.DestinationChannelId = channelId;
                bubblingObject.DestinationPointId = pointId;
                bubblingObject.IdComponent = idComponent;

                LogEngine.WriteLog(ConfigurationBag.EngineName,
                    $"SendNullMessageOffRamp bubblingObject.SenderChannelId {bubblingObject.SenderChannelId} " +
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
                    Constant.LogLevelInformation);

                // Queue the data
                OffRampEngineQueue.Enqueue(bubblingObject);

                Trace.WriteLine(
                    $"Sent Message Type: {messageType} - To ChannelID: {channelId} PointID: {pointId}");
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


        /// <summary>
        ///     TODO The off ramp engine on publish.
        /// </summary>
        /// <param name="bubblingObjects"></param>
        private static void OffRampEngineQueueOnPublish(List<BubblingObject> bubblingObjects)
        {
            foreach (var bubblingObject in bubblingObjects)
            {
                Trace.WriteLine($"INGEST MESSAGE!.");
                //todo optimization ho messo la ricezione per la ottimizzatione decommenta  // OffRampStream.SendMessage(bubblingObject);
                if (bubblingObject.LocalEvent)
                    MessageIngestor.IngestMessagge(bubblingObject);
                else
                {
                    // Send message to message provider 
                    OffRampStream.SendMessage(bubblingObject);
                }
            }
        }
    }
}