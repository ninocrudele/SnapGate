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
using System.IO;
using System.Reflection;
using System.ServiceModel.Web;
using System.Xml;
using Newtonsoft.Json;
using SnapGate.Framework.Base;
using SnapGate.Framework.Common;
using SnapGate.Framework.CompressionLibrary;
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Engine.OffRamp;
using SnapGate.Framework.Log;
using SnapGate.Framework.Serialization.Xml;
using Formatting = Newtonsoft.Json.Formatting;

#endregion

namespace SnapGate.Framework.Engine
{
    /// <summary>
    ///     class concerning the syncronization engine
    /// </summary>
    public static class SyncProvider
    {
        /// <summary>
        ///     The on syncronization.
        /// </summary>
        public static bool OnSyncronization;

        /// <summary>
        ///     Start restart o send the message
        /// </summary>
        internal static void RestartBecauseSync()
        {
            // TODO to complete 
        }


        /// <summary>
        ///     Method execute by REST to push the syncronization bag out to other points
        /// </summary>
        public static void SendSyncPush(string destinationChannelId, string destinationPointId)
        {
            try
            {
                byte[] content =
                    Helpers.CreateFromDirectory(
                        ConfigurationBag.Configuration.DirectoryOperativeRootExeName);

                BubblingObject bubblingObject = new BubblingObject(content);
                bubblingObject.MessageType = "SyncPush";
                bubblingObject.SenderPointId = ConfigurationBag.Configuration.PointId;
                bubblingObject.SenderChannelId = ConfigurationBag.Configuration.ChannelId;

                OffRampEngineSending.SendMessageOffRamp(bubblingObject,
                    "SyncPush",
                    destinationChannelId,
                    destinationPointId,
                    string.Empty);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(ConfigurationBag.EngineName,
                    "Configuration.WebApiEndPoint key empty, internal Web Api interface disable",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelWarning);
            }
        }

        /// <summary>
        ///     Method execute by REST to push the syncronization bag out to other points
        /// </summary>
        public static void SendSyncPull(string destinationChannelId, string destinationPointId)
        {
            try
            {
                BubblingObject bubblingObject = new BubblingObject(null);
                bubblingObject.MessageType = "SyncPull";
                OffRampEngineSending.SendMessageOffRamp(bubblingObject,
                    "SyncPull",
                    destinationChannelId,
                    destinationPointId,
                    string.Empty);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(ConfigurationBag.EngineName,
                    "Configuration.WebApiEndPoint key empty, internal Web Api interface disable",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelWarning);
            }
        }

        /// <summary>
        ///     The sync write configuration.
        /// </summary>
        /// <param name="channelId">
        ///     The channel id.
        /// </param>
        /// <param name="pointId">
        ///     The point id.
        /// </param>
        /// <param name="configurationFile">
        ///     The configuration file.
        /// </param>
        public static void SyncWriteConfiguration(string channelId, string pointId, byte[] configurationFile)
        {
            try
            {
                Trace.WriteLine(
                    $"Received configuration from ChannelID: {channelId} PointID {pointId}");
                var folder = string.Concat(ConfigurationBag.SyncDirectoryGcPoints(), "\\", channelId, "\\", pointId);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                File.WriteAllBytes(Path.Combine(folder, "GCPointConfiguration.xml"), configurationFile);
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

        /// <summary>
        ///     The sync local bubbling configuration file.
        /// </summary>
        /// <param name="syncConfigurationFile">
        ///     The sync configuration file.
        /// </param>
        public static void SyncLocalBubblingConfigurationFile(SyncConfigurationFile syncConfigurationFile)
        {
            try
            {
                Trace.WriteLine(
                    $"Syncronize configuration file. {syncConfigurationFile.Name}");
                var filename = Path.GetFileName(syncConfigurationFile.Name);

                var configurationFile = string.Concat(
                    syncConfigurationFile.FileType == "Trigger"
                        ? ConfigurationBag.DirectoryBubblingTriggers()
                        : ConfigurationBag.DirectoryBubblingEvents(),
                    "\\",
                    filename);
                File.WriteAllBytes(configurationFile, syncConfigurationFile.FileContent);
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

        /// <summary>
        ///     Sync a single configuration file
        /// </summary>
        /// <param name="syncConfigurationFile">
        ///     The sync Configuration File.
        /// </param>
        /// <param name="messageType">
        ///     The Message Type.
        /// </param>
        /// <param name="senderId">
        ///     The Sender ID.
        /// </param>
        /// <param name="senderName">
        ///     The Sender Name.
        /// </param>
        /// <param name="senderDescriprion">
        ///     The Sender Descriprion.
        /// </param>
        /// <param name="channelId">
        ///     The Channel ID.
        /// </param>
        /// <param name="channelName">
        ///     The Channel Name.
        /// </param>
        /// <param name="channelDescription">
        ///     The Channel Description.
        /// </param>
        public static void SyncBubblingConfigurationFile(
            SyncConfigurationFile syncConfigurationFile,
            string messageType,
            string senderId,
            string senderName,
            string senderDescriprion,
            string channelId,
            string channelName,
            string channelDescription)
        {
            try
            {
                Trace.WriteLine(
                    $"Syncronize configuration file. {syncConfigurationFile.Name} from PointID {senderId} ");

                var gcPointInfo = string.Concat(
                    channelId,
                    "^",
                    channelName,
                    "^",
                    channelDescription,
                    "^",
                    senderId,
                    "^",
                    senderName,
                    "^",
                    senderDescriprion);
                var filename = Path.GetFileName(syncConfigurationFile.Name);

                var configurationFile = string.Concat(
                    ConfigurationBag.SyncDirectoryGcPoints(),
                    "\\",
                    channelId,
                    "\\",
                    senderId,
                    "\\",
                    syncConfigurationFile.FileType == "Trigger"
                        ? ConfigurationBag.DirectoryNameTriggers
                        : ConfigurationBag.DirectoryNameEvents,
                    "\\",
                    filename);
                var gcPointInfoFile = string.Concat(
                    ConfigurationBag.SyncDirectoryGcPoints(),
                    "\\",
                    channelId,
                    "\\",
                    senderId,
                    "\\",
                    ConfigurationBag.DirectoryFileNameTriggerEventInfo);
                var directory = Path.GetDirectoryName(configurationFile);
                if (directory != null && !Directory.Exists(directory))
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Directory.CreateDirectory(directory);
                }
                else
                {
                    LogEngine.WriteLog(ConfigurationBag.EngineName,
                        $"Error in {MethodBase.GetCurrentMethod().Name} - Missing configuration file.",
                        Constant.LogLevelError,
                        Constant.TaskCategoriesError,
                        null,
                        Constant.LogLevelError);
                }

                File.WriteAllText(gcPointInfoFile, gcPointInfo);

                File.WriteAllBytes(configurationFile, syncConfigurationFile.FileContent);
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

        // Sync all the configuration  event and triggers files
        /// <summary>
        ///     TODO The sync bubbling configuration file list.
        /// </summary>
        /// <param name="syncConfigurationFileList">
        ///     TODO The sync configuration file list.
        /// </param>
        /// <param name="messageType">
        ///     TODO The message type.
        /// </param>
        /// <param name="senderId">
        ///     TODO The sender id.
        /// </param>
        /// <param name="senderName">
        ///     TODO The sender name.
        /// </param>
        /// <param name="senderDescriprion">
        ///     TODO The sender descriprion.
        /// </param>
        /// <param name="channelId">
        ///     TODO The channel id.
        /// </param>
        /// <param name="channelName">
        ///     TODO The channel name.
        /// </param>
        /// <param name="channelDescription">
        ///     TODO The channel description.
        /// </param>
        public static void SyncBubblingConfigurationFileList(
            List<SyncConfigurationFile> syncConfigurationFileList,
            string messageType,
            string senderId,
            string senderName,
            string senderDescriprion,
            string channelId,
            string channelName,
            string channelDescription)
        {
            try
            {
                foreach (var syncConfigurationFile in syncConfigurationFileList)
                {
                    SyncBubblingConfigurationFile(
                        syncConfigurationFile,
                        messageType,
                        senderId,
                        senderName,
                        senderDescriprion,
                        channelId,
                        channelName,
                        channelDescription);
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

        // Sync all the configuration  event and triggers files
        /// <summary>
        ///     TODO The sync local bubbling configuration file.
        /// </summary>
        /// <param name="syncConfigurationFileList">
        ///     TODO The sync configuration file list.
        /// </param>
        public static void SyncLocalBubblingConfigurationFile(List<SyncConfigurationFile> syncConfigurationFileList)
        {
            try
            {
                foreach (var syncConfigurationFile in syncConfigurationFileList)
                {
                    SyncLocalBubblingConfigurationFile(syncConfigurationFile);
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

        /// <summary>
        ///     Send all Bubbling configuration
        /// </summary>
        /// <param name="channelId">
        ///     The Channel ID.
        /// </param>
        /// <param name="pointId">
        ///     The Point ID.
        /// </param>
        public static void SyncSendBubblingConfiguration(string channelId, string pointId)
        {
            try
            {
                Trace.WriteLine(
                    $"Syncronizing  Bubbling confuguration - Point ID {ConfigurationBag.Configuration.PointId}");
                //todo optimization adesso usi il bubblingobject per trasporto, serializza il tutto e mettilo the datastorage del bubblingobject
                //OffRampEngineSending.SendMessageOnRamp(
                //    EventsEngine.SyncronizationConfigurationFileList,
                //    "SyncSendBubblingConfiguration", 
                //    channelId, 
                //    pointId, 
                //    null);
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

        /// <summary>
        ///     Send all configuration
        /// </summary>
        /// <param name="channelId">
        ///     The Channel ID.
        /// </param>
        /// <param name="pointId">
        ///     The Point ID.
        /// </param>
        public static void SyncSendConfiguration(string channelId, string pointId)
        {
            try
            {
                Trace.WriteLine(
                    $"Send all confuguration - Point ID {ConfigurationBag.Configuration.PointId}");
                var stream = GetConfiguration();

                // byte[] data
                // SerializationEngine.ObjectToByteArray(stream);
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    //todo optimization adesso usi il bubblingobject per trasporto, serializza il tutto e mettilo the datastorage del bubblingobject
                    //OffRampEngineSending.SendMessageOnRamp(
                    //    memoryStream.ToArray(), 
                    //    "SyncSendConfiguration", 
                    //    channelId, 
                    //    pointId, 
                    //    null);
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

        /// <summary>
        ///     TODO The sync send file bubbling configuration.
        /// </summary>
        /// <param name="channelId">
        ///     TODO The channel id.
        /// </param>
        /// <param name="pointId">
        ///     TODO The point id.
        /// </param>
        /// <param name="fileName">
        ///     TODO The file name.
        /// </param>
        /// <param name="messageType">
        ///     TODO The message type.
        /// </param>
        public static void SyncSendFileBubblingConfiguration(
            string channelId,
            string pointId,
            string fileName,
            string messageType)
        {
            try
            {
                Trace.WriteLine(
                    $"Syncronizing  Bubbling confuguration - Point ID {ConfigurationBag.Configuration.PointId}");
                var triggerConfigurationList = new List<SyncConfigurationFile>();

                try
                {
                    var folder = Path.Combine(
                        ConfigurationBag.SyncDirectoryGcPoints(),
                        channelId,
                        pointId,
                        messageType == "Trigger"
                            ? ConfigurationBag.DirectoryNameTriggers
                            : ConfigurationBag.DirectoryNameEvents);

                    var content = File.ReadAllBytes(Path.Combine(folder, fileName));

                    //todo optimization adesso usi il bubblingobject per trasporto, serializza il tutto e mettilo the datastorage del bubblingobject
                    //var syncConfigurationFile = new SyncConfigurationFile(
                    //    messageType, 
                    //    fileName, 
                    //    content, 
                    //    string.Empty);
                    //triggerConfigurationList.Add(syncConfigurationFile);
                    //OffRampEngineSending.SendMessageOnRamp(
                    //    triggerConfigurationList, 
                    //    "SyncSendFileBubblingConfiguration", 
                    //    channelId, 
                    //    pointId, 
                    //    null);
                }
                catch (Exception ex)
                {
                    LogEngine.WriteLog(
                        ConfigurationBag.EngineName,
                        $"Error in {MethodBase.GetCurrentMethod().Name} File {fileName}",
                        Constant.LogLevelError,
                        Constant.TaskCategoriesError,
                        ex,
                        Constant.LogLevelError);
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

        /// <summary>
        ///     Request all the bubbling configuration
        /// </summary>
        /// <param name="channelId">
        /// </param>
        /// <param name="pointId">
        /// </param>
        public static void SyncSendRequestBubblingConfiguration(string channelId, string pointId)
        {
            try
            {
                Trace.WriteLine(
                    $"Syncronizing  Bubbling confuguration - Point ID {ConfigurationBag.Configuration.PointId}");
                OffRampEngineSending.SendNullMessageOffRamp(
                    "SyncSendRequestBubblingConfiguration",
                    channelId,
                    pointId,
                    string.Empty,
                    null,
                    null);
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

        /// <summary>
        ///     Request all the configuration
        /// </summary>
        /// <param name="channelId">
        /// </param>
        /// <param name="pointId">
        /// </param>
        public static void SyncSendRequestConfiguration(string channelId, string pointId)
        {
            try
            {
                Trace.WriteLine(
                    $"Syncronizing  Bubbling confuguration - Point ID {ConfigurationBag.Configuration.PointId}");
                OffRampEngineSending.SendNullMessageOffRamp(
                    "SyncSendRequestConfiguration",
                    channelId,
                    pointId,
                    string.Empty,
                    null,
                    null);
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

        /// <summary>
        ///     The sync update component.
        /// </summary>
        /// <param name="channelId">
        ///     The channel id.
        /// </param>
        /// <param name="pointId">
        ///     The point id.
        /// </param>
        /// <param name="bubblingObject">
        ///     The bubbling event.
        /// </param>
        public static void SyncUpdateComponent(string channelId, string pointId, BubblingObject bubblingObject)
        {
            //try
            //{
            //    var assemblyToUpdate = Path.ChangeExtension(
            //        bubblingObject.AssemblyFile, 
            //        Configuration.UpdateFileExtension);

            //    var path = string.Empty;
            //    if (bubblingObject.BubblingEventType == BubblingEventType.Trigger)
            //    {
            //        path = Path.Combine(ConfigurationBag.DirectoryTriggers(), assemblyToUpdate);
            //    }

            //    if (bubblingObject.BubblingEventType == BubblingEventType.Event)
            //    {
            //        path = Path.Combine(ConfigurationBag.DirectoryEvents(), assemblyToUpdate);
            //    }

            //    // Path.Combine(ConfigurationBag.DirectoryBubblingTriggers()
            //    File.WriteAllBytes(path, bubblingObject.AssemblyContent);
            //    ServiceStates.RestartNeeded = true;
            //    Trace.WriteLine(
            //        $"Received IDComponent {bubblingObject.IdComponent} from Point ID {pointId}", 
            //        ConsoleColor.Yellow);
            //}
            //catch (Exception ex)
            //{
            //    LogEngine.WriteLog(
            //        ConfigurationBag.EngineName, 
            //        $"Error in {MethodBase.GetCurrentMethod().Name}", 
            //        Constant.LogLevelError, 
            //        Constant.TaskCategoriesError, 
            //        ex, 
            //        Constant.LogLevelError);
            //}
        }

        /// <summary>
        ///     The sync send component.
        /// </summary>
        /// <param name="channelId">
        ///     The channel id.
        /// </param>
        /// <param name="pointId">
        ///     The point id.
        /// </param>
        /// <param name="idComponent">
        ///     The id component.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string SyncSendComponent(string channelId, string pointId, string idComponent)
        {
            //try
            //{
            //    Trace.WriteLine(
            //        $"Send IDComponent {idComponent} to Point ID {pointId}", 
            //        ConsoleColor.Yellow);

            //    var component =
            //        (from components in EventsEngine.GlobalAssemblyFiles
            //         where components.IdComponent == idComponent
            //         select components).First();
            //    string ret;
            //    if (component != null)
            //    {
            //        OffRampEngineSending.SendMessageOnRamp(
            //            component, 
            //            ConfigurationBag.MessageDataProperty.SyncSendComponent, 
            //            channelId, 
            //            pointId, 
            //            null,
            //            null);
            //        ret = $"Sent IDComponent {idComponent} to Point ID {pointId}";
            //    }
            //    else
            //    {
            //        ret = $"IDComponent {idComponent} not present.";
            //    }

            //    return ret;
            //}
            //catch (Exception ex)
            //{
            //    LogEngine.WriteLog(
            //        ConfigurationBag.EngineName, 
            //        $"Error in {MethodBase.GetCurrentMethod().Name}", 
            //        Constant.LogLevelError, 
            //        Constant.TaskCategoriesError, 
            //        ex, 
            //        Constant.LogLevelError);
            //    return ex.Message;
            //}
            return "";
        }

        /// <summary>
        ///     Request all the bubbling configuration
        /// </summary>
        /// <param name="channelId">
        /// </param>
        /// <param name="pointId">
        /// </param>
        /// <param name="idComponent">
        ///     The ID Component.
        /// </param>
        public static void SyncSendRequestComponent(string channelId, string pointId, string idComponent)
        {
            try
            {
                Trace.WriteLine(
                    $"Send request for IDComponent {idComponent} to Point ID {pointId}");
                OffRampEngineSending.SendNullMessageOffRamp(
                    "SyncSendRequestComponent",
                    channelId,
                    pointId,
                    idComponent,
                    null,
                    null);
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

        /// <summary>
        ///     Return the complete configuration
        /// </summary>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        public static Stream GetConfiguration()
        {
            try
            {
                // TODO [Next version] Implement swagger definition too
                var docMain = new XmlDocument();

                // Configuration node
                var eConfiguration = docMain.CreateElement(string.Empty, "Configuration", string.Empty);

                // Configurtation root node
                // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                XmlHelpers.AddAttribute(docMain, eConfiguration, "DateTimeNow", DateTime.Now.ToString());

                // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                XmlHelpers.AddAttribute(docMain, eConfiguration, "DateTimeUtcNow", DateTime.UtcNow.ToString());
                XmlHelpers.AddAttribute(docMain, eConfiguration, "ChannelId", ConfigurationBag.Configuration.ChannelId);
                XmlHelpers.AddAttribute(docMain, eConfiguration, "ChannelName",
                    ConfigurationBag.Configuration.ChannelName);
                XmlHelpers.AddAttribute(
                    docMain,
                    eConfiguration,
                    "ChannelDescription",
                    ConfigurationBag.Configuration.ChannelDescription);
                XmlHelpers.AddAttribute(docMain, eConfiguration, "PointId", ConfigurationBag.Configuration.PointId);
                XmlHelpers.AddAttribute(docMain, eConfiguration, "PointName", ConfigurationBag.Configuration.PointName);
                XmlHelpers.AddAttribute(docMain, eConfiguration, "PointDescription",
                    ConfigurationBag.Configuration.PointDescription);

                // Bubbling Triggers
                var bubbling = docMain.CreateElement(string.Empty, "Bubbling", string.Empty);
                eConfiguration.AppendChild(bubbling);

                var bubblingTriggers = docMain.CreateElement(string.Empty, "Triggers", string.Empty);
                bubbling.AppendChild(bubblingTriggers);

                foreach (var triggerConfiguration in EventsEngine.ConfigurationJsonTriggerFileList)
                {
                    var eTrigger = docMain.CreateElement(string.Empty, "Trigger", string.Empty);
                    bubblingTriggers.AppendChild(eTrigger);

                    XmlHelpers.AddAttribute(docMain, eTrigger, "IdComponent", triggerConfiguration.Trigger.IdComponent);
                    XmlHelpers.AddAttribute(docMain, eTrigger, "Name", triggerConfiguration.Trigger.Name);
                    XmlHelpers.AddAttribute(docMain, eTrigger, "Description", triggerConfiguration.Trigger.Description);

                    // Properties
                    var properties = docMain.CreateElement(string.Empty, "TriggerProperties", string.Empty);
                    eTrigger.AppendChild(properties);
                    if (triggerConfiguration.Trigger.TriggerProperties != null)
                    {
                        foreach (var property in triggerConfiguration.Trigger.TriggerProperties)
                        {
                            if (property.Name != "DataContext")
                            {
                                var xeProperty = docMain.CreateElement(string.Empty, "TriggerProperty", string.Empty);
                                properties.AppendChild(xeProperty);

                                XmlHelpers.AddAttribute(docMain, xeProperty, "Name", property.Name);
                                XmlHelpers.AddAttribute(docMain, xeProperty, "Value", property.Value.ToString());
                            }
                        }
                    }

                    // Events
                    // ReSharper disable once InconsistentNaming
                    var Events = docMain.CreateElement(string.Empty, "Events", string.Empty);
                    eTrigger.AppendChild(Events);
                    foreach (var _event in triggerConfiguration.Events)
                    {
                        var Event = docMain.CreateElement(string.Empty, "Event", string.Empty);
                        Events.AppendChild(Event);

                        XmlHelpers.AddAttribute(docMain, Event, "IdComponent", _event.IdComponent);
                        XmlHelpers.AddAttribute(docMain, Event, "IdConfiguration", _event.IdConfiguration);
                        XmlHelpers.AddAttribute(docMain, Event, "Name", _event.Name);
                        XmlHelpers.AddAttribute(docMain, Event, "Description", _event.Description);

                        // 1*
                        if (_event.Channels != null)
                        {
                            var channels = docMain.CreateElement(string.Empty, "Channels", string.Empty);
                            Event.AppendChild(channels);

                            // ReSharper disable once InconsistentNaming
                            foreach (var _channel in _event.Channels)
                            {
                                var channel = docMain.CreateElement(string.Empty, "Channel", string.Empty);
                                channels.AppendChild(channel);
                                XmlHelpers.AddAttribute(docMain, channel, "ChannelId", _channel.ChannelId);
                                XmlHelpers.AddAttribute(docMain, channel, "Name", _channel.ChannelName);
                                XmlHelpers.AddAttribute(docMain, channel, "Description", _channel.ChannelDescription);

                                // ReSharper disable once InconsistentNaming
                                foreach (var _point in _channel.Points)
                                {
                                    var point = docMain.CreateElement(string.Empty, "Point", string.Empty);
                                    channel.AppendChild(point);
                                    XmlHelpers.AddAttribute(docMain, point, "PointId", _point.PointId);
                                    XmlHelpers.AddAttribute(docMain, channel, "Name", _point.Name);
                                    XmlHelpers.AddAttribute(docMain, channel, "Description", _point.Description);
                                }
                            }
                        }

                        if (_event.Correlation != null)
                        {
                            var correlations = docMain.CreateElement(string.Empty, "Correlation", string.Empty);
                            Event.AppendChild(correlations);
                            var endPointsCorrelation = docMain.CreateElement(string.Empty, "EndPointIDs", string.Empty);
                            correlations.AppendChild(endPointsCorrelation);

                            var correlationChannels = docMain.CreateElement(string.Empty, "Channels", string.Empty);
                            Event.AppendChild(correlationChannels);

                            // ReSharper disable once InconsistentNaming
                            foreach (var _channel in _event.Channels)
                            {
                                var channel = docMain.CreateElement(string.Empty, "Channel", string.Empty);
                                correlationChannels.AppendChild(channel);
                                XmlHelpers.AddAttribute(docMain, channel, "ChannelId", _channel.ChannelId);
                                XmlHelpers.AddAttribute(docMain, channel, "Name", _channel.ChannelName);
                                XmlHelpers.AddAttribute(docMain, channel, "Description", _channel.ChannelDescription);

                                // ReSharper disable once InconsistentNaming
                                foreach (var _point in _channel.Points)
                                {
                                    var point = docMain.CreateElement(string.Empty, "Point", string.Empty);
                                    channel.AppendChild(channel);
                                    XmlHelpers.AddAttribute(docMain, point, "PointId", _point.PointId);
                                    XmlHelpers.AddAttribute(docMain, channel, "Name", _point.Name);
                                    XmlHelpers.AddAttribute(docMain, channel, "Description", _point.Description);
                                }
                            }

                            endPointsCorrelation.AppendChild(correlationChannels);

                            XmlHelpers.AddAttribute(docMain, Event, "Name", _event.Correlation.Name);
                            XmlHelpers.AddAttribute(docMain, Event, "ScriptRule", _event.Correlation.ScriptRule);

                            // ReSharper disable once UnusedVariable
                            foreach (var eventCorrelated in _event.Correlation.Events)
                            {
                                var eventCorrelation = docMain.CreateElement(string.Empty, "Events", string.Empty);
                                correlations.AppendChild(eventCorrelation);

                                XmlHelpers.AddAttribute(docMain, eventCorrelation, "IdComponent", _event.IdComponent);
                                XmlHelpers.AddAttribute(
                                    docMain,
                                    eventCorrelation,
                                    "IDConfiguration",
                                    _event.IdConfiguration);
                                XmlHelpers.AddAttribute(docMain, eventCorrelation, "Name", _event.Name);
                                XmlHelpers.AddAttribute(docMain, eventCorrelation, "Description", _event.Description);
                            }
                        }
                    }

                    {
                        // json configuration
                        var jsonTemplate = docMain.CreateElement(string.Empty, "JSON", string.Empty);
                        eTrigger.AppendChild(jsonTemplate);
                        var jsonSerialization = JsonConvert.SerializeObject(
                            triggerConfiguration,
                            Formatting.Indented,
                            new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
                        var jsonText = docMain.CreateTextNode(jsonSerialization);
                        jsonTemplate.AppendChild(jsonText);
                    }
                }

                // Bubbling Triggers
                // ************************************************************

                // ************************************************************
                // Bubbling Events
                var bubblingEvents = docMain.CreateElement(string.Empty, "Events", string.Empty);
                bubbling.AppendChild(bubblingEvents);

                foreach (var eventConfigurationItem in EventsEngine.ConfigurationJsonEventFileList)
                {
                    var eventConfiguration = eventConfigurationItem.Value;
                    var eEventBubbling = docMain.CreateElement(string.Empty, "Event", string.Empty);
                    bubblingEvents.AppendChild(eEventBubbling);

                    XmlHelpers.AddAttribute(
                        docMain,
                        eEventBubbling,
                        "IdConfiguration",
                        eventConfiguration.Event.IdConfiguration);
                    XmlHelpers.AddAttribute(
                        docMain,
                        eEventBubbling,
                        "IdComponent",
                        eventConfiguration.Event.IdComponent);
                    XmlHelpers.AddAttribute(docMain, eEventBubbling, "Name", eventConfiguration.Event.Name);
                    XmlHelpers.AddAttribute(
                        docMain,
                        eEventBubbling,
                        "Description",
                        eventConfiguration.Event.Description);

                    if (eventConfiguration.Event.EventProperties != null)
                    {
                        // Properties
                        var propertiesEvent = docMain.CreateElement(string.Empty, "EventProperties", string.Empty);
                        eEventBubbling.AppendChild(propertiesEvent);
                        foreach (var property in eventConfiguration.Event.EventProperties)
                        {
                            if (property.Name != "DataContext")
                            {
                                var xeProperty = docMain.CreateElement(string.Empty, "EventProperty", string.Empty);
                                propertiesEvent.AppendChild(xeProperty);

                                XmlHelpers.AddAttribute(docMain, xeProperty, "Name", property.Name);
                                XmlHelpers.AddAttribute(docMain, xeProperty, "Value", property.Value.ToString());
                            }
                        }
                    }

                    {
                        // json Template
                        var jsonTemplate = docMain.CreateElement(string.Empty, "JSON", string.Empty);
                        bubblingEvents.AppendChild(jsonTemplate);
                        var jsonSerialization = JsonConvert.SerializeObject(
                            eventConfiguration,
                            Formatting.Indented,
                            new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
                        var jsonText = docMain.CreateTextNode(jsonSerialization);
                        jsonTemplate.AppendChild(jsonText);
                    }

                    // Bubbling Events
                    // ************************************************************
                }

                // ************************************************************
                // Triggers

                var eTriggers = docMain.CreateElement(string.Empty, "Triggers", string.Empty);

                eConfiguration.AppendChild(eTriggers);

                foreach (var triggerAssembly in EventsEngine.CacheTriggerComponents)
                {
                    var eTrigger = docMain.CreateElement(string.Empty, "Trigger", string.Empty);
                    eTriggers.AppendChild(eTrigger);

                    XmlHelpers.AddAttribute(docMain, eTrigger, "IdComponent", triggerAssembly.Value.Id);
                    XmlHelpers.AddAttribute(docMain, eTrigger, "Name", triggerAssembly.Value.Name);
                    XmlHelpers.AddAttribute(docMain, eTrigger, "Description", triggerAssembly.Value.Description);
                    XmlHelpers.AddAttribute(docMain, eTrigger, "Version", triggerAssembly.Value.Version.ToString());
                    XmlHelpers.AddAttribute(docMain, eTrigger, "Shared", triggerAssembly.Value.Shared);
                    XmlHelpers.AddAttribute(
                        docMain,
                        eTrigger,
                        "PollingRequired",
                        triggerAssembly.Value.PollingRequired);
                    XmlHelpers.AddAttribute(docMain, eTrigger, "AssemblyFile", triggerAssembly.Value.AssemblyFile);

                    // Actions
                    var properties = docMain.CreateElement(string.Empty, "Properties", string.Empty);
                    eTrigger.AppendChild(properties);
                    if (triggerAssembly.Value.Properties.Count > 1)
                    {
                        foreach (var property in triggerAssembly.Value.Properties)
                        {
                            if (property.Value.Name != "DataContext")
                            {
                                var xeProperty = docMain.CreateElement(string.Empty, "Property", string.Empty);
                                properties.AppendChild(xeProperty);

                                XmlHelpers.AddAttribute(docMain, xeProperty, "Name", property.Value.Name);
                                XmlHelpers.AddAttribute(docMain, xeProperty, "Description", property.Value.Description);
                                XmlHelpers.AddAttribute(
                                    docMain,
                                    xeProperty,
                                    "Type",
                                    property.Value.AssemblyPropertyInfo.PropertyType.ToString());
                            }
                        }
                    }

                    // Actions
                    //if (triggerAssembly.BaseActions.Count > 1)
                    //{
                    //    var actions = docMain.CreateElement(string.Empty, "Actions", string.Empty);
                    //    eTrigger.AppendChild(actions);
                    //    foreach (var baseAction in triggerAssembly.BaseActions)
                    //    {
                    //        var action = docMain.CreateElement(string.Empty, "Action", string.Empty);
                    //        actions.AppendChild(action);

                    //        XmlHelpers.AddAttribute(docMain, action, "Id", baseAction.Id);
                    //        XmlHelpers.AddAttribute(docMain, action, "Name", baseAction.Name);
                    //        XmlHelpers.AddAttribute(docMain, action, "Description", baseAction.Description);

                    //        var parameters = docMain.CreateElement(string.Empty, "Parameters", string.Empty);

                    //        // ReSharper disable once InconsistentNaming
                    //        foreach (var Parameter in baseAction.Parameters)
                    //        {
                    //            var parameter = docMain.CreateElement(string.Empty, "Parameter", string.Empty);
                    //            parameters.AppendChild(parameter);
                    //            XmlHelpers.AddAttribute(docMain, parameter, "Name", parameter.Name);
                    //        }
                    //    }
                    //}
                    {
                        // json Template
                        var jsonTemplate = docMain.CreateElement(string.Empty, "JSON", string.Empty);
                        eTrigger.AppendChild(jsonTemplate);
                        var jsonSerialization =
                            SerializationHelper.CreteJsonTriggerConfigurationTemplate(triggerAssembly.Value);
                        var jsonText = docMain.CreateTextNode(jsonSerialization);
                        jsonTemplate.AppendChild(jsonText);
                    }
                }

                // ************************************************************
                // Events

                var eEvents = docMain.CreateElement(string.Empty, "Events", string.Empty);

                eConfiguration.AppendChild(eEvents);

                foreach (var eventAssembly in EventsEngine.CacheEventComponents)
                {
                    var eEvent = docMain.CreateElement(string.Empty, "Event", string.Empty);
                    eEvents.AppendChild(eEvent);

                    XmlHelpers.AddAttribute(docMain, eEvent, "IdComponent", eventAssembly.Value.Id);
                    XmlHelpers.AddAttribute(docMain, eEvent, "Name", eventAssembly.Value.Name);
                    XmlHelpers.AddAttribute(docMain, eEvent, "Description", eventAssembly.Value.Description);
                    XmlHelpers.AddAttribute(docMain, eEvent, "Version", eventAssembly.Value.Version.ToString());
                    XmlHelpers.AddAttribute(docMain, eEvent, "Shared", eventAssembly.Value.Shared);
                    XmlHelpers.AddAttribute(
                        docMain,
                        eEvent,
                        "PollingRequired",
                        eventAssembly.Value.PollingRequired);
                    XmlHelpers.AddAttribute(docMain, eEvent, "AssemblyFile", eventAssembly.Value.AssemblyFile);

                    // Actions
                    var properties = docMain.CreateElement(string.Empty, "Properties", string.Empty);
                    eEvent.AppendChild(properties);
                    if (eventAssembly.Value.Properties.Count > 1)
                    {
                        foreach (var property in eventAssembly.Value.Properties)
                        {
                            if (property.Value.Name != "DataContext")
                            {
                                var xeProperty = docMain.CreateElement(string.Empty, "Property", string.Empty);
                                properties.AppendChild(xeProperty);

                                XmlHelpers.AddAttribute(docMain, xeProperty, "Name", property.Value.Name);
                                XmlHelpers.AddAttribute(docMain, xeProperty, "Description", property.Value.Description);
                                XmlHelpers.AddAttribute(
                                    docMain,
                                    xeProperty,
                                    "Type",
                                    property.Value.AssemblyPropertyInfo.PropertyType.ToString());
                            }
                        }
                    }

                    {
                        // json Template
                        var jsonTemplate = docMain.CreateElement(string.Empty, "JSON", string.Empty);
                        eEvent.AppendChild(jsonTemplate);
                        var jsonSerialization =
                            SerializationHelper.CreteJsonEventConfigurationTemplate(eventAssembly.Value);
                        var jsonText = docMain.CreateTextNode(jsonSerialization);
                        jsonTemplate.AppendChild(jsonText);
                    }
                }

                // If called by web context
                if (WebOperationContext.Current != null)
                {
                    var currentWebContext = WebOperationContext.Current;
                    if (currentWebContext != null)
                    {
                        currentWebContext.OutgoingResponse.ContentType = "text/xml";
                    }
                }

                return new MemoryStream(EncodingDecoding.EncodingString2Bytes(eConfiguration.OuterXml));
            }
            catch (Exception ex)
            {
                var docMain = new XmlDocument();
                var errorTemplate = docMain.CreateElement(string.Empty, "Error", string.Empty);
                var errorText = docMain.CreateTextNode(ex.Message);
                errorTemplate.AppendChild(errorText);

                var currentWebContext = WebOperationContext.Current;
                currentWebContext.OutgoingResponse.ContentType = "text/xml";
                return new MemoryStream(EncodingDecoding.EncodingString2Bytes(errorTemplate.OuterXml));
            }
        }

        // *********************************************************************************************
        // Configuration files AREA
        // *********************************************************************************************
        /// <summary>
        ///     TODO The refresh bubbling setting.
        /// </summary>
        public static void RefreshBubblingSetting(bool RefreshTriggerRunning)
        {
            ////Check if it's a simple event/trigger configuration update (NOT DLL), then just update the eventlistsetting
            Trace.WriteLine("-!EVENTS CONFIGURATION ENGINE SYNC!-");
            EventsEngine.RefreshBubblingSetting(RefreshTriggerRunning);
            ConfigurationBag.LoadConfiguration();
        }
    }
}