// --------------------------------------------------------------------------------------------------
// <copyright file = "MessageIngestor.cs" company="Nino Crudele">
//   Copyright (c) 2015 Nino Crudele. All Rights Reserved.
// </copyright>
// <summary>
//    Author: Nino Crudele
//    Blog:   http://ninocrudele.me
//    Email:  nino.crudele@live.com
//    Info:   http://grabcaster.io/
// 
//    Unless explicitly acquired and licensed from Licensor under another
//    license, the contents of this file are subject to the Reciprocal Public
//    License ("RPL") Version 1.5, or subsequent versions as allowed by the RPL,
//    and You may not copy or use this file in either source code or executable
//    form, except in compliance with the terms and conditions of the RPL.
//    
//    All software distributed under the RPL is provided strictly on an "AS
//    IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, AND
//    LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT
//    LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//    PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the RPL for specific
//    language governing rights and limitations under the RPL. 
//    
//    The Reciprocal Public License 1.5 (RPL1.5) license is described here: 
//    http://www.opensource.org/licenses/rpl1.5.txt
//  </summary>
// --------------------------------------------------------------------------------------------------
namespace GrabCaster.Framework.Engine.OnRamp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using GrabCaster.Framework.Base;
    using GrabCaster.Framework.Common;
    using GrabCaster.Framework.Contracts.Attributes;
    using GrabCaster.Framework.Contracts.Bubbling;
    using GrabCaster.Framework.Contracts.Messaging;
    using GrabCaster.Framework.Engine;
    using GrabCaster.Framework.Engine.OffRamp;
    using GrabCaster.Framework.Log;
    using GrabCaster.Framework.Serialization;
    using GrabCaster.Framework.Serialization.Object;
    using GrabCaster.Framework.Storage;

    using Microsoft.Data.Edm;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.WindowsAzure.Storage.Blob;

    using Newtonsoft.Json;

    /// <summary>
    /// Engine main message ingestor
    /// </summary>
    public static class MessageIngestor
    {
        public delegate void SetConsoleActionEventEmbedded(string DestinationConsolePointId, ISkeletonMessage skeletonMessage);
        /// <summary>
        /// Used internally by the embedded
        /// </summary>
        public static SetConsoleActionEventEmbedded setConsoleActionEventEmbedded { get; set; }


        private static object classInstanceDpp;
        private static bool secondaryPersistProviderEnabled;
        private static int secondaryPersistProviderByteSize;
        private static MethodInfo methodPersistEventFromBlob;
        private static readonly object[] ParametersPersistEventFromBlob = { null };

        public static void Init()
        {
            try
            {

                secondaryPersistProviderEnabled = Configuration.SecondaryPersistProviderEnabled();
                secondaryPersistProviderByteSize = Configuration.SecondaryPersistProviderByteSize();

                // Load the abrstracte persistent provider
                var devicePersistentProviderComponent = Path.Combine(
                                    Configuration.DirectoryOperativeRootExeName(),
                                    Configuration.PersistentProviderComponent());

                // Create the reflection method cached 
                var assemblyPersist = Assembly.LoadFrom(devicePersistentProviderComponent);

                // Main class logging
                var assemblyClassDpp = (from t in assemblyPersist.GetTypes()
                                        let attributes = t.GetCustomAttributes(typeof(DevicePersistentProviderContract), true)
                                        where t.IsClass && attributes != null && attributes.Length > 0
                                        select t).First();

                var classAttributeDpp = assemblyClassDpp.GetCustomAttributes(typeof(DevicePersistentProviderContract), true);

                if (classAttributeDpp.Length > 0)
                {
                    Debug.WriteLine("DevicePersistentProviderContract - methodPersistEvent caller");
                    methodPersistEventFromBlob = assemblyClassDpp.GetMethod("PersistEventFromStorage");
                   
                }

                classInstanceDpp = Activator.CreateInstance(assemblyClassDpp, null);

            }
            catch (Exception ex)
            {

                LogEngine.WriteLog(
                    Configuration.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.ErrorEventIdHighCriticalEventHubs,
                    Constant.TaskCategoriesEventHubs,
                    ex,
                    EventLogEntryType.Error);
            }
        }
        public static void IngestMessagge(object message)
        {
            string senderId;
            string senderDescription;
            byte[] eventDataByte = null;
            var skeletonMessage = (ISkeletonMessage)message;

            // ****************************IF MESSAGE TYPE = GRABCASTER*************************
            try
            {
                // Check message subscription, it must come from engine
                if (skeletonMessage.Properties[Configuration.GrabCasterMessageTypeName].ToString()
                    != Configuration.GrabCasterMessageTypeValue)
                {
                    LogEngine.ConsoleWriteLine(
                        "Not GrabCaster message type received -DISCARED-",
                        ConsoleColor.DarkYellow);
                    return;
                }
                else
                {
                    // Who sent the message
                    senderId = skeletonMessage.Properties[Configuration.MessageDataProperty.SenderId.ToString()].ToString();
                    senderDescription =
                        skeletonMessage.Properties[Configuration.MessageDataProperty.SenderDescriprion.ToString()].ToString();

                    //If using Guid pattern as string some system put an escape character like \ before the end brachet }

                    // Who receive the message
                    LogEngine.ConsoleWriteLine(
                        $"Event received from Sender {senderId} Sender description {senderDescription}",
                        ConsoleColor.DarkCyan);

                    // ****************************IF SAME SENDER*************************
                    if (senderId == Configuration.PointId())
                    {
                        LogEngine.ConsoleWriteLine("Same sender ID event discared.", ConsoleColor.Green);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                // If error then not message typeof (no property present.)
                LogEngine.WriteLog(
                    Configuration.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name} - Not GrabCaster message type received (Missing GrabCaster_MessageType_Name properties.) -DISCARED-",
                    Constant.ErrorEventIdHighCritical,
                    Constant.TaskCategoriesError,
                    ex,
                    EventLogEntryType.Error);
                return;
            }

            try
            {


                // ****************************GET FROM STORAGE IF REQUIRED*************************
                if ((bool)skeletonMessage.Properties[Configuration.MessageDataProperty.Persisting.ToString()])
                {
                    ParametersPersistEventFromBlob[0] = skeletonMessage.Properties[Configuration.MessageDataProperty.MessageId.ToString()];
                    var ret = methodPersistEventFromBlob.Invoke(classInstanceDpp, ParametersPersistEventFromBlob);
                    eventDataByte = (byte[])ret;
                }
                else
                {
                    eventDataByte = skeletonMessage.Body;
                }

                //*******************************************************************
                //              3 IF TYPES EVENT - CONSOLE - REST
                // first area events, second console, third rest
                //*******************************************************************

                // ****************************IF EVENT TYPE*************************
                if (skeletonMessage.Properties[Configuration.MessageDataProperty.MessageType.ToString()].ToString()
                    == Configuration.MessageDataProperty.Event.ToString())
                {

                    // ****************************IF EMBEDED TYPE EXECUTE TRIGGER*************************
                    if (skeletonMessage.Properties[Configuration.MessageDataProperty.Embedded.ToString()].ToString()
                        == "true")
                    {
                        var recChannelId =
                            skeletonMessage.Properties[Configuration.MessageDataProperty.ReceiverChannelId.ToString()].ToString();
                        var recPointId =
                            skeletonMessage.Properties[Configuration.MessageDataProperty.ReceiverPointId.ToString()].ToString();

                        //If using Guid pattern as string some system put an escape character like \ before the end brachet }

                        var reqAvailable = (recChannelId.Contains(Configuration.ChannelId())
                                                && recPointId.Contains(Configuration.PointId()))
                                               || (recChannelId.Contains(Configuration.ChannelAll)
                                                   && recPointId.Contains(Configuration.PointId()))
                                               || (recChannelId.Contains(Configuration.ChannelId())
                                                   && recPointId.Contains(Configuration.ChannelAll))
                                               || (recChannelId.Contains(Configuration.ChannelAll)
                                                   && recPointId.Contains(Configuration.ChannelAll));

                        if (!reqAvailable)
                        {
                            return;
                        }
                        string idConfiguration =
                            skeletonMessage.Properties[Configuration.MessageDataProperty.IdConfiguration.ToString()].ToString();
                        string idComponent =
                            skeletonMessage.Properties[Configuration.MessageDataProperty.IdComponent.ToString()].ToString();

                        try
                        {
                            var triggerSingleInstance =
                                (from trigger in EventsEngine.BubblingTriggerConfigurationsSingleInstance
                                 where trigger.IdComponent == idComponent && trigger.IdConfiguration == idConfiguration
                                 select trigger).First();
                            var bubblingTriggerConfiguration = triggerSingleInstance;
                            LogEngine.ConsoleWriteLine($"Execute trigger idConfiguration {idConfiguration} and idComponent {idComponent}", ConsoleColor.Green);
                            EventsEngine.ExecuteTriggerConfiguration(bubblingTriggerConfiguration, skeletonMessage.Body);

                        }
                        catch (Exception ex)
                        {
                            LogEngine.WriteLog(Configuration.EngineName,
                                            $"Error in {MethodBase.GetCurrentMethod().Name} - ExecuteTriggerConfiguration Error - Missing the idConfiguration {idConfiguration} and idComponent {idComponent}",
                                            Constant.ErrorEventIdHighCritical,
                                            Constant.TaskCategoriesError,
                                            ex,
                                            EventLogEntryType.Error);
                        }


                        // ****************************IF EMBEDED RETURN HERE*************************
                        return;
                    }

                    // ****************************CAST TO BUBBLING EVENT*************************
                    var eventBubbling = (BubblingEvent)SerializationEngine.ByteArrayToObject(eventDataByte);

                    // ****************************PERSIST MESSAGE IN FOLDER*************************
                    PersistentProvider.PersistMessage(eventBubbling, PersistentProvider.CommunicationDiretion.OffRamp);

                    if (Configuration.LoggingVerbose())
                    {
                        var serializedEvents = JsonConvert.SerializeObject(
                            eventBubbling.Events,
                            Formatting.Indented,
                            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                        LogEngine.ConsoleWriteLine(
                            $"Event received from point id: {skeletonMessage.Properties[Configuration.MessageDataProperty.SenderId.ToString()]} -point name : {skeletonMessage.Properties[Configuration.MessageDataProperty.SenderDescriprion.ToString()]} - {serializedEvents}",
                            ConsoleColor.Green);
                    }

                    // ****************************IF EXIST EVENT TO EXECUTE*************************
                    var eventsAvailable = from eventbubble in eventBubbling.Events
                                          from channel in eventbubble.Channels
                                          from point in channel.Points
                                          where
                                              (channel.ChannelId == Configuration.ChannelId()
                                               && point.PointId == Configuration.PointId())
                                              || (channel.ChannelId == Configuration.ChannelAll
                                                  && point.PointId == Configuration.PointId())
                                              || (channel.ChannelId == Configuration.ChannelId()
                                                  && point.PointId == Configuration.PointAll)
                                              || (channel.ChannelId == Configuration.ChannelAll
                                                  && point.PointId == Configuration.PointAll)
                                          select eventbubble;


                    if (!eventsAvailable.Any())
                    {
                        // ****************************NO EVENT RETURN*************************
                        return;
                    }

                    // ****************************EVENT EXIST EXECUTE*************************
                    EventsEngine.ExecuteBubblingActionEvent(
                        eventBubbling,
                        false,
                        skeletonMessage.Properties[Configuration.MessageDataProperty.SenderId.ToString()].ToString());
                    return;
                }



                var receiverChannelId =
                skeletonMessage.Properties[Configuration.MessageDataProperty.ReceiverChannelId.ToString()].ToString();
                var receiverPointId =
                    skeletonMessage.Properties[Configuration.MessageDataProperty.ReceiverPointId.ToString()].ToString();

                var requestAvailable = (receiverChannelId == Configuration.ChannelId()
                                        && receiverPointId == Configuration.PointId())
                                       || (receiverChannelId == Configuration.ChannelAll
                                           && receiverPointId == Configuration.PointId())
                                       || (receiverChannelId == Configuration.ChannelId()
                                           && receiverPointId == Configuration.PointAll)
                                       || (receiverChannelId == Configuration.ChannelAll
                                           && receiverPointId == Configuration.PointAll);

                if (!requestAvailable)
                {
                    // ****************************NOT FOR ME*************************
                    return;
                }


                // **************************** IF CONSOLE TYPE *************************
                //Save in a string to simplify the reading and code
                string OperationTypRequested =
                    skeletonMessage.Properties[Configuration.MessageDataProperty.MessageType.ToString()].ToString();

                //******************* OPERATION CONF BAG- ALL THE CONF FILES AND DLLS ****************************************************************
                if (OperationTypRequested
                    == Configuration.MessageDataProperty.ConsoleSendBubblingBag.ToString() ||
                    OperationTypRequested
                    == Configuration.MessageDataProperty.ConsoleSendBubblingBagFromPoint.ToString())
                {

                    //If send or received operaion
                    bool sendOperation = true && OperationTypRequested == Configuration.MessageDataProperty.ConsoleSendBubblingBag.ToString();
                    if (!Configuration.DisableDeviceProviderInterface())
                    {
                        if(sendOperation)
                                OffRampEngineSending.SendMessageOnRamp(
                                                    EventsEngine.bubblingBag,
                                                    Configuration.MessageDataProperty.ConsoleSendBubblingBagFromPoint,
                                                    skeletonMessage.Properties[Configuration.MessageDataProperty.ChannelId.ToString()].ToString(),
                                                    skeletonMessage.Properties[Configuration.MessageDataProperty.SenderId.ToString()].ToString(),
                                                    null,
                                                    null);
                        else
                        {
                            setConsoleActionEventEmbedded(
                                Configuration.MessageDataProperty.ReceiverPointId.ToString(),
                                skeletonMessage);
                        }

                    }
                    else
                    {
                        LogEngine.WriteLog(
                            Configuration.EngineName,
                            "Warning the Device Provider Interface is disable, the GrabCaster point will be able to work in local mode only.",
                            Constant.ErrorEventIdHighCritical,
                            Constant.TaskCategoriesError,
                            null,
                            EventLogEntryType.Warning);
                    }
                }
                //******************* OPERATION CONF BAG- ALL THE CONF FILES AND DLLS ****************************************************************





                // **************************** REST SERVICE CALLS AREA *************************

                //RICEVE I BUBBLINGS
                // ****************************WRITE THE TRIGGERS AND EVENTS CONFIGURATION FILES IN FOLDERS*************************
                if (skeletonMessage.Properties[Configuration.MessageDataProperty.MessageType.ToString()].ToString()
                    == Configuration.MessageDataProperty.SyncSendBubblingConfiguration.ToString())
                {
                    // eventDataByte = NULL
                    LogEngine.ConsoleWriteLine(
                        $"SyncSendBubblingConfiguration from - {senderId} - {senderDescription}",
                        ConsoleColor.Cyan);
                    var syncConfigurationFilelIst =
                        (List<SyncConfigurationFile>)SerializationEngine.ByteArrayToObject(eventDataByte);
                    SyncProvider.SyncBubblingConfigurationFileList(
                        syncConfigurationFilelIst,
                        skeletonMessage.Properties[Configuration.MessageDataProperty.MessageType.ToString()].ToString(),
                        skeletonMessage.Properties[Configuration.MessageDataProperty.SenderId.ToString()].ToString(),
                        skeletonMessage.Properties[Configuration.MessageDataProperty.SenderName.ToString()].ToString(),
                        skeletonMessage.Properties[Configuration.MessageDataProperty.SenderDescriprion.ToString()].ToString(),
                        skeletonMessage.Properties[Configuration.MessageDataProperty.ChannelId.ToString()].ToString(),
                        skeletonMessage.Properties[Configuration.MessageDataProperty.ChannelName.ToString()].ToString(),
                        skeletonMessage.Properties[Configuration.MessageDataProperty.ChannelDescription.ToString()].ToString());
                    return;
                }

                //RICEVE LA CONF DAL PUNTO
                // ****************************WRITE THE POINT CONFIGURATION FILE IN FOLDERs*************************
                if (skeletonMessage.Properties[Configuration.MessageDataProperty.MessageType.ToString()].ToString()
                    == Configuration.MessageDataProperty.SyncSendConfiguration.ToString())
                {
                    // EventDataByte = NULL
                    LogEngine.ConsoleWriteLine(
                        $"SyncSendConfiguration from - {senderId} - {senderDescription}",
                        ConsoleColor.Cyan);
                    SyncProvider.SyncWriteConfiguration(
                        skeletonMessage.Properties[Configuration.MessageDataProperty.ChannelId.ToString()].ToString(),
                        skeletonMessage.Properties[Configuration.MessageDataProperty.SenderId.ToString()].ToString(),
                        eventDataByte);
                    return;
                }

                //RICEVE E SCRIVE UN FILE DI CONFIGURAZIONE TRIGGER O EVENT NELLA BUBBLIG
                // ****************************WRITE THE SPECIFIC BUBBLING FILE IN FOLDERS*************************
                if (skeletonMessage.Properties[Configuration.MessageDataProperty.MessageType.ToString()].ToString()
                    == Configuration.MessageDataProperty.SyncSendFileBubblingConfiguration.ToString())
                {
                    // EventDataByte = NULL
                    LogEngine.ConsoleWriteLine(
                        $"SyncSendFileBubblingConfiguration from - {senderId} - {senderDescription}",
                        ConsoleColor.Cyan);
                    var syncConfigurationFilelIst =
                        (List<SyncConfigurationFile>)SerializationEngine.ByteArrayToObject(eventDataByte);
                    SyncProvider.SyncLocalBubblingConfigurationFile(syncConfigurationFilelIst);
                    return;
                }


                //SCRIVE UN COMP DLL IN EVENTO O TRIGGER FOLDER
                // ****************************UPDATE A DLL COMPONENT IN FILE IN FOLDERS*************************
                if (skeletonMessage.Properties[Configuration.MessageDataProperty.MessageType.ToString()].ToString()
                    == Configuration.MessageDataProperty.SyncSendComponent.ToString())
                {
                    // EventDataByte = NULL
                    LogEngine.ConsoleWriteLine(
                        $"SyncSendComponent from - {senderId} - {senderDescription}",
                        ConsoleColor.Cyan);
                    var eventBubbling = (BubblingEvent)SerializationEngine.ByteArrayToObject(eventDataByte);
                    SyncProvider.SyncUpdateComponent(
                        skeletonMessage.Properties[Configuration.MessageDataProperty.ChannelId.ToString()].ToString(),
                        skeletonMessage.Properties[Configuration.MessageDataProperty.SenderId.ToString()].ToString(),
                        eventBubbling);
                    return;
                }

                // Request to send the local configuration
                // Send the configuration
                if (skeletonMessage.Properties[Configuration.MessageDataProperty.MessageType.ToString()].ToString()
                    == Configuration.MessageDataProperty.SyncSendRequestBubblingConfiguration.ToString())
                {
                    // EventDataByte = NULL
                    LogEngine.ConsoleWriteLine(
                        $"SyncSendBubblingConfiguration from - {senderId} - {senderDescription}",
                        ConsoleColor.Cyan);
                    SyncProvider.SyncSendBubblingConfiguration(
                        skeletonMessage.Properties[Configuration.MessageDataProperty.ChannelId.ToString()].ToString(),
                        skeletonMessage.Properties[Configuration.MessageDataProperty.SenderId.ToString()].ToString());
                    return;
                }

                // Request to send the  configuration
                // Send the configuration
                if (skeletonMessage.Properties[Configuration.MessageDataProperty.MessageType.ToString()].ToString()
                    == Configuration.MessageDataProperty.SyncSendRequestConfiguration.ToString())
                {
                    // EventDataByte = NULL
                    LogEngine.ConsoleWriteLine(
                        $"SyncSendRequestConfiguration from - {senderId} - {senderDescription}",
                        ConsoleColor.Cyan);

                    SyncProvider.SyncSendConfiguration(skeletonMessage.Properties[Configuration.MessageDataProperty.ChannelId.ToString()].ToString(),
                                                        skeletonMessage.Properties[Configuration.MessageDataProperty.SenderId.ToString()].ToString());


                    return;
                }


                // Request to send back a component
                if (skeletonMessage.Properties[Configuration.MessageDataProperty.MessageType.ToString()].ToString()
                    == Configuration.MessageDataProperty.SyncSendRequestComponent.ToString())
                {
                    // EventDataByte = NULL
                    LogEngine.ConsoleWriteLine(
                        $"SyncSendRequestComponent from - {senderId} - {senderDescription}",
                        ConsoleColor.Cyan);
                    SyncProvider.SyncSendComponent(
                        skeletonMessage.Properties[Configuration.MessageDataProperty.ChannelId.ToString()].ToString(),
                        skeletonMessage.Properties[Configuration.MessageDataProperty.SenderId.ToString()].ToString(),
                        skeletonMessage.Properties[Configuration.MessageDataProperty.IdComponent.ToString()].ToString());
                    return;
                }

            }
            catch (Exception ex)
            {


                LogEngine.WriteLog(Configuration.EngineName,
                                  $"Error in {MethodBase.GetCurrentMethod().Name}",
                                  Constant.ErrorEventIdHighCritical,
                                  Constant.TaskCategoriesError,
                                  ex,
                                  EventLogEntryType.Error);




            }
        }
    }
}