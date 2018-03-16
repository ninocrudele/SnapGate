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
using System.Runtime.Serialization;
using System.Text;
using Microsoft.ServiceBus;
using Newtonsoft.Json;

#endregion

namespace SnapGate.Framework.Base
{
    public enum EhReceivePatternType
    {
        Direct,

        Abstract
    }

    public enum EventHubsCheckPointPattern
    {
        CheckPoint,

        Dt,

        Dtepoch,

        Dtutcnow,

        Dtnow,

        Dtutcnowepoch,

        Dtnowepoch
    }

    /// <summary>
    ///     Configuration
    /// </summary>
    public static class ConfigurationBag
    {
        //Configuration storagew
        //Abstraction layer for the configuration storage, now is using json file
        public static Configuration Configuration;

        //General vars
        public static string FlagFileNameSyncToDo = "Sync.todo";

        public static string GcEventsConfigurationFilesExtensions = @".evn|.trg|.off";

        public static string GcEventsFilesExtensions = @".dll|.evn|.trg|.off";

        public static string EngineName = "SnapGate";

        public static string CloneFileExtension = "clone";

        public static string UpdateFileExtension = "update";

        public static string DllFileExtension = "dll";

        public static string BlobDllSyncPostfix = "components";

        //public static string General_RunTimeFile = "runtime";
        public static string ConfigurationFileExtension = ".cfg";

        //Directories

        public static string DirectoryNameConfigurationRoot = "Root";

        public static string DirectoryFileNameTriggerEventInfo = "GCPoint.info";

        public static string DirectoryNameTriggers = "Triggers";

        public static string DirectoryNameEvents = "Events";

        public static string DirectoryNameComponents = "Components";

        public static string DirectoryNameChains = "Chains";

        public static string DirectoryNameBubbling = "Bubbling";

        public static string DirectoryNameDeployment = "Deploy";

        public static string DirectoryNamePublishing = "Publishing";

        public static string DirectoryNamePoints = "GCPoints";

        public static string DirectoryNameSync = "Sync";

        public static string DirectoryNameIn = "In";

        public static string DirectoryNameOut = "Out";

        //Messages
        public static string MessageContextAll = "*";

        public static string MessageFileStorageExtension = @".gcm";

        //Channels
        public static string ChannelAll = "*";

        //Points
        public static string PointAll = "*";

        //Events
        public static string TriggersDllExtension = @".(dll)";

        public static string DeployExtension = @".(deploy)";

        public static string EventsDllExtension = @".(dll)";

        public static string ComponentsDllExtension = @".(dll)";

        public static string TriggersUpdateExtension = @".(update)";

        public static string EventsUpdateExtension = @".(update)";

        public static string TriggersDllExtensionLookFor = "*.dll";

        public static string DeployExtensionLookFor = ".trigger|.event|.component|";

        public static string EventsDllExtensionLookFor = "*.dll";

        public static string ComponentsDllExtensionLookFor = "*.dll";

        public static string GenericDllExtensionLookFor = "*.dll";

        public static string TriggersUpdateExtensionLookFor = "*.update";

        public static string EventsUpdateExtensionLookFor = "*.update";

        public static string BubblingTriggersEventsDllExtension = @".dll";

        public static string BubblingTriggersExtension = @".trg";

        public static string BubblingEventsExtension = @".evn";

        public static string BubblingComponentsExtension = @".cmp";

        public static string BubblingChainsExtension = @".chain";

        public static string BubblingOffExtension = @".off";

        public static string configurationFile = string.Empty;

        //Methods
        public static void LoadConfiguration()
        {
            try
            {
                //Get Exe name
                var filename =
                    Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName)
                        .Replace(".vshost", "");

                //Get the configuration file
                configurationFile = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    string.Concat(filename.Replace(".vshost", ""), ConfigurationFileExtension));
                Trace.WriteLine("ConfigurationFile:", configurationFile);

                //byte[] contentClear = null;
                //if (AESEncryption.SecurityOn_Aes())
                //{
                //    byte[] content = File.ReadAllBytes(configurationFile);
                //}


                string defaultFile = configurationFile.Replace("SnapGate.cfg", "DevDefault.cfg");

                if (File.Exists(defaultFile))
                {
                    string newContent = string.Empty;
                    string content = File.ReadAllText(defaultFile);

                    Console.WriteLine("SnapGate first Run");
                    Console.WriteLine("Insert a point name and press Enter:");
                    string pointName = Console.ReadLine();
                    newContent = content.Replace("POINTID", Guid.NewGuid().ToString().ToUpper());
                    newContent = newContent.Replace("POINTNAME", pointName);


                    File.WriteAllText(defaultFile, newContent);
                    File.Move(defaultFile, configurationFile);
                }

                Configuration =
                    JsonConvert.DeserializeObject<Configuration>(
                        Encoding.UTF8.GetString(File.ReadAllBytes(configurationFile)));

                //Check Cluster configuration
                if (Configuration.Clustered)
                {
                    Configuration.BaseDirectory = Configuration.ClusterBaseFolder;
                    if (!Directory.Exists(Configuration.BaseDirectory))
                    {
                        throw new NotImplementedException(
                            $"Missing the Cluster Base Folder Directory {Configuration.BaseDirectory}.");
                    }
                }
                else
                {
                    Configuration.BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }

                var rootDirConf = Path.Combine(
                    Configuration.BaseDirectory,
                    string.Concat(DirectoryNameConfigurationRoot, "_", filename));

                if (!Directory.Exists(rootDirConf))
                {
                    EventLog.WriteEntry("SnapGate", $"Missing the Configuration Directory {rootDirConf}.",
                        EventLogEntryType.Error);
                    throw new NotImplementedException($"Missing the Configuration Directory {rootDirConf}.");
                }

                //AppDomain.CurrentDomain.BaseDirectory
                Configuration.DirectoryOperativeRootExeName = Path.Combine(
                    Configuration.BaseDirectory,
                    string.Concat(DirectoryNameConfigurationRoot, "_", filename));
                Configuration.DirectoryServiceExecutable = Configuration.BaseDirectory;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("SnapGate", ex.Message, EventLogEntryType.Error);
                throw ex;
            }
        }

        public static void SaveConfgurtation(Configuration configuration)
        {
            var filename = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);
            var configurationFile = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                string.Concat(filename.Replace(".vshost", ""), ConfigurationFileExtension));
            Trace.WriteLine("ConfigurationFile:", configurationFile);
            var configurationStorageContent = JsonConvert.SerializeObject(configuration);

            File.WriteAllText(configurationFile, configurationStorageContent);
        }

        public static bool IamConsole()
        {
            bool iamconsole = Path.GetFileName(configurationFile) == "SnapGateUI.cfg";
            return iamconsole;
        }


        /// <summary>
        ///     BUBBLING directory
        /// </summary>
        /// <returns></returns>
        public static string DirectoryBubbling()
        {
            return Path.Combine(Configuration.DirectoryOperativeRootExeName, DirectoryNameBubbling);
        }


        /// <summary>
        ///     BUBBLING directory
        /// </summary>
        /// <returns></returns>
        public static string DirectoryDeployment()
        {
            return Path.Combine(Configuration.DirectoryOperativeRootExeName, DirectoryNameDeployment);
        }


        /// <summary>
        ///     BUBBLING\Log directory
        /// </summary>
        /// <returns></returns>
        public static string DirectoryLog()
        {
            return Path.Combine(Configuration.BaseDirectory, "Log");
        }

        /// <summary>
        ///     BUBBLING\Log\Concole directory
        /// </summary>
        /// <returns></returns>
        public static string DirectoryLogConsole()
        {
            return Path.Combine(Configuration.BaseDirectory, "Log\\Console");
        }

        /// <summary>
        ///     ENDPOINTS directory
        /// </summary>
        /// <returns></returns>
        public static string DirectoryEndPoints()
        {
            return Path.Combine(Configuration.DirectoryOperativeRootExeName, DirectoryNamePoints);
        }

        /// <summary>
        ///     TRIGGERS directory
        /// </summary>
        /// <returns></returns>
        public static string DirectoryTriggers()
        {
            return Path.Combine(Configuration.DirectoryOperativeRootExeName, DirectoryNameTriggers);
        }

        /// <summary>
        ///     EVENTS directory
        /// </summary>
        /// <returns></returns>
        public static string DirectoryEvents()
        {
            return Path.Combine(Configuration.DirectoryOperativeRootExeName, DirectoryNameEvents);
        }

        /// <summary>
        ///     EVENTS directory
        /// </summary>
        /// <returns></returns>
        public static string DirectoryComponents()
        {
            return Path.Combine(Configuration.DirectoryOperativeRootExeName, DirectoryNameComponents);
        }

        /// <summary>
        ///     ROOT\BUBBLING\TRIGGERS
        /// </summary>
        /// <returns></returns>
        public static string DirectoryBubblingTriggers()
        {
            return Path.Combine(
                Configuration.DirectoryOperativeRootExeName,
                DirectoryNameBubbling,
                DirectoryNameTriggers);
        }

        /// <summary>
        ///     BUBBLING\EVENTS direcotry
        /// </summary>
        /// <returns></returns>
        public static string DirectoryBubblingEvents()
        {
            return Path.Combine(
                Configuration.DirectoryOperativeRootExeName,
                DirectoryNameBubbling,
                DirectoryNameEvents);
        }

        /// <summary>
        ///     BUBBLING\EVENTS direcotry
        /// </summary>
        /// <returns></returns>
        public static string DirectoryBubblingComponents()
        {
            return Path.Combine(
                Configuration.DirectoryOperativeRootExeName,
                DirectoryNameBubbling,
                DirectoryNameComponents);
        }

        /// <summary>
        ///     BUBBLING\EVENTS direcotry
        /// </summary>
        /// <returns></returns>
        public static string DirectoryBubblingChains()
        {
            return Path.Combine(
                Configuration.DirectoryOperativeRootExeName,
                DirectoryNameBubbling,
                DirectoryNameChains);
        }


        /// <summary>
        ///     Connection string of storage
        /// </summary>
        /// <returns></returns>
        public static string ReturnStorageConnectionString()
        {
            return
                $"DefaultEndpointsProtocol=https;AccountName={Configuration.GroupStorageAccountName};AccountKey={Configuration.GroupStorageAccountKey}";
        }


        //Syncronization Area


        /// <summary>
        ///     ENDPOINTS directory
        /// </summary>
        /// <returns></returns>
        public static string SyncDirectoryGcPoints()
        {
            return Path.Combine(Configuration.BaseDirectory, DirectoryNamePoints);
        }

        public static string SyncBuildSpecificDirectoryGcPoints(string PointId)
        {
            return Path.Combine(Configuration.BaseDirectory, DirectoryNamePoints, PointId);
        }

        public static string SyncBuildSpecificDirectoryGcPointsIn(string PointId)
        {
            return Path.Combine(Configuration.BaseDirectory, DirectoryNamePoints, PointId, DirectoryNameIn);
        }

        public static string SyncBuildSpecificDirectoryGcPointsOut(string PointId)
        {
            return Path.Combine(Configuration.BaseDirectory, DirectoryNamePoints, PointId, DirectoryNameOut);
        }

        public static string SyncDirectorySync()
        {
            return Path.Combine(Configuration.BaseDirectory, DirectoryNameSync);
        }

        public static string SyncDirectorySyncIn()
        {
            return Path.Combine(Configuration.BaseDirectory, DirectoryNameSync, DirectoryNameIn);
        }

        public static string SyncDirectorySyncOut()
        {
            return Path.Combine(Configuration.BaseDirectory, DirectoryNameSync, DirectoryNameOut);
        }
    }

    [DataContract]
    [Serializable]
    public class Configuration
    {
        [DataMember] public bool RunInternalPolling { get; set; }

        [DataMember] public int LoggingLevel { get; set; }

        [DataMember] public bool Clustered { get; set; }

        [DataMember] public string ClusterBaseFolder { get; set; }

        [DataMember] public bool SecondaryPersistProviderEnabled { get; set; }

        [DataMember] public int SecondaryPersistProviderByteSize { get; set; }

        [DataMember] public string PersistentProviderComponent { get; set; }

        [DataMember] public string EventsStreamComponent { get; set; }

        [DataMember] public bool RunLocalOny { get; set; }

        [DataMember] public string LoggingComponent { get; set; }

        [DataMember] public string LoggingComponentStorage { get; set; }

        [DataMember] public EhReceivePatternType EventHubsReceivingPattern { get; set; }

        /// <summary>
        ///     Wait time before restart
        /// </summary>
        [DataMember]
        public int WaitTimeBeforeRestarting { get; set; }

        [DataMember] public string WebApiEndPoint { get; set; }

        /// <summary>
        ///     Enable persisting message engine
        /// </summary>
        /// <returns></returns>
        public bool EnablePersistingMessaging { get; set; }

        public string DirectoryServiceExecutable { get; set; }

        public string DirectoryOperativeRootExeName { get; set; }

        public string BaseDirectory { get; set; }

        [DataMember] public bool PowerBIEnabled { get; set; }

        [DataMember] public bool CleanPowerBIDataset { get; set; }

        [DataMember] public string HAGroup { get; set; }

        //how many seconds for each check
        [DataMember] public int HACheckTime { get; set; }

        //how many seconds to be considered still active 
        [DataMember] public int HAInactivity { get; set; }


        //Main Azure connection string
        [DataMember] public string AzureNameSpaceConnectionString { get; set; }

        [DataMember] public string RedisConnectionString { get; set; }

        /// <summary>
        ///     EventHub name used by station
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string GroupEventHubsName { get; set; }

        /// <returns></returns>
        /// <summary>
        ///     Group Azure Storage Account name
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string GroupStorageAccountName { get; set; }

        /// <summary>
        ///     Group Storage Account key
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string GroupStorageAccountKey { get; set; }

        /// <summary>
        ///     Local storage pattern used by engine
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string LocalStorageProvider { get; set; }

        /// <summary>
        ///     Local storage used by engine
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string LocalStorageConnectionString { get; set; }

        /// <summary>
        ///     Unique GUID point of the local engine
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string PointId { get; set; }

        /// <summary>
        ///     point name
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string PointName { get; set; }

        /// <summary>
        ///     point Description
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string PointDescription { get; set; }

        /// <summary>
        ///     Unique GUID channel of the local engine
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string ChannelId { get; set; }

        /// <summary>
        ///     Channel name
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string ChannelName { get; set; }

        /// <summary>
        ///     Engine Description
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string ChannelDescription { get; set; }

        /// <summary>
        ///     if the engine needs to syncronize in automation
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public bool AutoSyncronizationEnabled { get; set; }

        /// <summary>
        ///     Enable log
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public bool LoggingEngineEnabled { get; set; }

        /// <summary>
        ///     EncodingType
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public EncodingType EncodingType { get; set; }


        /// <summary>
        ///     Global Trigger polling time (Milliseconds)
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public int EnginePollingTime { get; set; }

        /// <summary>
        ///     Define the date time receiving time from Event Hubs
        ///     Values("26/07/2015 16:58:35")
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public string EventHubsStartingDateTimeReceiving { get; set; }

        /// <summary>
        ///     Define the Event Hubs epochs
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public long EventHubsEpoch { get; set; }

        [DataMember] public ConnectivityMode ServiceBusConnectivityMode { get; set; }

        /// <summary>
        ///     Define the Event Hubs checkpoint pattern
        /// </summary>
        /// <returns></returns>
        [DataMember]
        public EventHubsCheckPointPattern EventHubsCheckPointPattern { get; set; }

        [DataMember] public int ThrottlingOnRampIncomingRateNumber { get; set; }

        [DataMember] public int ThrottlingOnRampIncomingRateSeconds { get; set; }

        [DataMember] public int ThrottlingOffRampIncomingRateNumber { get; set; }

        [DataMember] public int ThrottlingOffRampIncomingRateSeconds { get; set; }

        [DataMember] public int ThrottlingConsoleLogIncomingRateNumber { get; set; }

        [DataMember] public int ThrottlingConsoleLogIncomingRateSeconds { get; set; }

        [DataMember] public int ThrottlingLsiLogIncomingRateNumber { get; set; }

        [DataMember] public int ThrottlingLsiLogIncomingRateSeconds { get; set; }

        [DataMember] public int MaxWorkerThreads { get; set; }

        [DataMember] public int MaxAsyncWorkerThreads { get; set; }

        [DataMember] public int MinWorkerThreads { get; set; }

        [DataMember] public int MinAsyncWorkerThreads { get; set; }
    }
}