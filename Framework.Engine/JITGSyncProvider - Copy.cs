#region License
// -----------------------------------------------------------------------
// Copyright (c) Antonino Crudele.  All Rights Reserved.  
// This work is registered with the UK Copyright Service.
// Registration No:284695248
// Licensed under the Reciprocal Public License 1.5 (RPL1.5) 
// See License.txt in the project root for license information.
// -----------------------------------------------------------------------
#endregion
namespace Framework
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Base;
    using Contracts;

    using Framework.Engine.OffRamp;

    /// <summary>
    ///     class concerning the syncronization engine
    /// </summary>
    internal static class SyncProvider_OLD
    {
        /// <summary>
        ///     Send the request message for the Synconization
        /// </summary>
        /// <param name="Partner">Partner request to send</param>
        public static void SyncSendSyncRequest()
        {
            try
            {
                OffRampEngine.SendNullMessageOnRamp(Configuration.MessageDataProperty.SyncRequest,
                    Configuration.MessageContext__All);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(Configuration.General_Source,
                    string.Format("Error in {0}", MethodBase.GetCurrentMethod().Name),
                    Constant.Error_EventID_High_Critical_,
                    Constant.TaskCategories_,
                    ex,
                    EventLogEntryType.Error);
            }
        }

        public static void SyncConfigurationFileListRequested()
        {
            try
            {
                EventUpStream.SendServiceNullMessage(
                    Configuration.MessageDataProperty.SyncConfigurationFileListRequested,
                    Configuration.MessageContext__All);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(Configuration.General_Source,
                    string.Format("Error in {0}", MethodBase.GetCurrentMethod().Name),
                    Constant.Error_EventID_High_Critical_,
                    Constant.TaskCategories_,
                    ex,
                    EventLogEntryType.Error);
            }
        }

        /// <summary>
        ///     Send the request message for the Synconization
        /// </summary>
        /// <param name="Partner">Partner request to send</param>
        public static void SyncSendSyncAvailable()
        {
            try
            {
                EventUpStream.SendServiceNullMessage(Configuration.MessageDataProperty.SyncAvailable,
                    Configuration.MessageContext__All);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(Configuration.General_Source,
                    string.Format("Error in {0}", MethodBase.GetCurrentMethod().Name),
                    Constant.Error_EventID_High_Critical_,
                    Constant.TaskCategories_,
                    ex,
                    EventLogEntryType.Error);
            }
        }

        /// <summary>
        ///     Send local shared dll for the sync to the partner request
        /// </summary>
        /// <param name="Partner">Partner request to send</param>
        public static void SyncSendLocalDLL(AssemblyEvent jitgAssemblyEvent, string DestinationPartner)
        {
            try
            {
                EventsEngine.UpdateAssemblyEventListShared();
                LogEngine.ConsoleWriteLine(
                    string.Format("SENT EVENT - {0} to {1}", jitgAssemblyEvent.FileName, DestinationPartner),
                    ConsoleColor.Green);
                OffRampEngineSending.SendMessageOnRamp(jitgAssemblyEvent, Configuration.MessageDataProperty.SyncSendLocalDLL,
                    DestinationPartner, null);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(Configuration.General_Source,
                    string.Format("Error in {0}", MethodBase.GetCurrentMethod().Name),
                    Constant.Error_EventID_High_Critical_,
                    Constant.TaskCategories_,
                    ex,
                    EventLogEntryType.Error);
            }
        }

        /// <summary>
        ///     Send local shared dll for the sync to the partner request
        /// </summary>
        /// <param name="Partner">Partner request to send</param>
        public static void SyncSendLocalDLLList(string DestinationPartner)
        {
            try
            {
                EventsEngine.UpdateAssemblyEventListShared();
                foreach (var jitgAssemblyEvent in EventsEngine.AssemblyEventListShared)
                {
                    LogEngine.ConsoleWriteLine(
                        string.Format("SENT EVENT - {0} to {1}", jitgAssemblyEvent.FileName, DestinationPartner),
                        ConsoleColor.Green);
                    OffRampEngineSending.SendMessageOnRamp(jitgAssemblyEvent, Configuration.MessageDataProperty.SyncSendLocalDLL,
                        DestinationPartner, null);
                }
            }
            catch (InvalidOperationException eoe)
            {
                LogEngine.ConsoleWriteLine(
                    string.Format("Syncronization and Dictionaries rebuilded, Excpetion controlled - {0}", eoe.HResult),
                    ConsoleColor.DarkRed);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(Configuration.General_Source,
                    string.Format("Error in {0}", MethodBase.GetCurrentMethod().Name),
                    Constant.Error_EventID_High_Critical_,
                    Constant.TaskCategories_,
                    ex,
                    EventLogEntryType.Error);
            }
        }

        /// <summary>
        ///     Sync the local DLL trigger and events to the table storage
        /// </summary>
        public static void SyncLocalDLL(AssemblyEvent jitgAssemblyEvent)
        {
            try
            {
                var BubblingDirectory = Configuration.DirectoryOperativeRoot_ExeName();

                var assemblyFile = Path.Combine(BubblingDirectory, string.Concat(jitgAssemblyEvent.Type, "s"),
                    jitgAssemblyEvent.FileName);
                //Set the primary key, if equal then check the assembly version
                if (!File.Exists(assemblyFile))
                {
                    LogEngine.ConsoleWriteLine("-NEW EVENT RECEIVED-", ConsoleColor.Green);
                    File.WriteAllBytes(assemblyFile, jitgAssemblyEvent.AssemblyContent);
                    LogEngine.ConsoleWriteLine("-NEW EVENT CREATED-", ConsoleColor.Green);
                }
                else
                {
                    //To avoid the lock
                    //Before delete all clones
                    LogEngine.ConsoleWriteLine("-SYNC EVENT-", ConsoleColor.Green);

                    var assembly = Assembly.LoadFile(assemblyFile);

                    var versionOnLocal = assembly.GetName().Version;
                    var versionOnStorage = new Version(jitgAssemblyEvent.Version);

                    if (versionOnStorage.CompareTo(versionOnLocal) > 0)
                    {
                        LogEngine.ConsoleWriteLine(
                            string.Format("-SYNC EVENT TO DO- Local V.{0} Remote V. {1}", versionOnLocal,
                                versionOnStorage), ConsoleColor.Green);
                        var assemblyUpdate = Path.ChangeExtension(assemblyFile, Configuration.General_UpdateFile);
                        File.WriteAllBytes(assemblyUpdate, jitgAssemblyEvent.AssemblyContent);
                        LogEngine.ConsoleWriteLine("-SYNC EVENT DONE-", ConsoleColor.Green);
                    }
                    assembly = null;
                }
            }
            catch (IOException ioException)
            {
                LogEngine.ConsoleWriteLine("-SYNC EVENT -", ConsoleColor.Red);
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                LogEngine.ConsoleWriteLine("-SYNC EVENT -", ConsoleColor.Red);
            }

            catch (Exception ex)
            {
                LogEngine.WriteLog(Configuration.General_Source,
                    string.Format("Error in {0}", MethodBase.GetCurrentMethod().Name),
                    Constant.Error_EventID_High_Critical_,
                    Constant.TaskCategories_,
                    ex,
                    EventLogEntryType.Error);
            }
        }

        /// <summary>
        ///     Sync a single configuration file
        /// </summary>
        public static void SyncLocalConfigurationFile(SyncConfigurationFile jitgSyncConfigurationFile)
        {
            try
            {
                //invia il file con tutta la di e metti nelle prop che e un evento o trigger e la dir, il contenuto nel body eventdata
                //fai il replace della dir

                var newFilenameDir =
                    jitgSyncConfigurationFile.Name.Replace(jitgSyncConfigurationFile.DirectoryOperativeRoot_ExeName, "");
                newFilenameDir = string.Concat(Configuration.DirectoryEndPoints(), "\\",
                    jitgSyncConfigurationFile.Name, newFilenameDir);
                Directory.CreateDirectory(Path.GetDirectoryName(newFilenameDir));

                File.WriteAllBytes(newFilenameDir, jitgSyncConfigurationFile.FileContent);
            }
            catch (IOException ioException)
            {
                LogEngine.ConsoleWriteLine("-SYNC CONF. BUBBLING -", ConsoleColor.Red);
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                LogEngine.ConsoleWriteLine("-SYNC CONF. BUBBLING -", ConsoleColor.Red);
            }

            catch (Exception ex)
            {
                LogEngine.WriteLog(Configuration.General_Source,
                    string.Format("Error in {0}", MethodBase.GetCurrentMethod().Name),
                    Constant.Error_EventID_High_Critical_,
                    Constant.TaskCategories_,
                    ex,
                    EventLogEntryType.Error);
            }
        }

        //Sync all the configuration  event and triggers files
        public static void SyncLocalConfigurationFileList(List<SyncConfigurationFile> SyncConfigurationFileList)
        {
            try
            {
                LogEngine.ConsoleWriteLine("-SYNC LOCAL EVT/TRG CONF.-", ConsoleColor.Green);
                foreach (var jitgSyncConfigurationFile in SyncConfigurationFileList)
                {
                    SyncLocalConfigurationFile(jitgSyncConfigurationFile);
                }
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(Configuration.General_Source,
                    string.Format("Error in {0}", MethodBase.GetCurrentMethod().Name),
                    Constant.Error_EventID_High_Critical_,
                    Constant.TaskCategories_,
                    ex,
                    EventLogEntryType.Error);
            }
        }

        //Send all the configuration bubbling event and triggers
        public static void SyncSendConfigurationFile(SyncConfigurationFile jitgSyncConfigurationFile,
            string DestinationPartner)
        {
            try
            {
                LogEngine.ConsoleWriteLine(
                    string.Format("SENT CONFIGURATION EVT/TRG CONF. FILE - to {0}", DestinationPartner),
                    ConsoleColor.Green);
                OffRampEngineSending.SendMessageOnRamp(jitgSyncConfigurationFile,
                    Configuration.MessageDataProperty.SyncConfigurationFileToDo, DestinationPartner, null);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(Configuration.General_Source,
                    string.Format("Error in {0}", MethodBase.GetCurrentMethod().Name),
                    Constant.Error_EventID_High_Critical_,
                    Constant.TaskCategories_,
                    ex,
                    EventLogEntryType.Error);
            }
        }

        //Send all the configuration bubbling event and triggers
        public static void SyncSendConfigurationFileList(string DestinationPartner)
        {
            try
            {
                LogEngine.ConsoleWriteLine(
                    string.Format("SENT BUBBLING EVT/TRG CONF. - to {0}", DestinationPartner), ConsoleColor.Yellow);
                OffRampEngineSending.SendMessageOnRamp(EventsEngine.SyncConfigurationFileList,
                    Configuration.MessageDataProperty.SyncConfigurationFileListToDo, DestinationPartner, null);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(Configuration.General_Source,
                    string.Format("Error in {0}", MethodBase.GetCurrentMethod().Name),
                    Constant.Error_EventID_High_Critical_,
                    Constant.TaskCategories_,
                    ex,
                    EventLogEntryType.Error);
            }
        }
    }
}