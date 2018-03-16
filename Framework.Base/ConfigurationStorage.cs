#region License
//-----------------------------------------------------------------------
// <copyright file="ConfigurationStorage.cs" company="Antonino Crudele">
//   Copyright (c) Antonino Crudele. All Rights Reserved.
// </copyright>
// <license>
//   This work is registered with the UK Copyright Service.
//   Registration No:284695248
//   Licensed under the Reciprocal Public License 1.5 (RPL1.5) 
//   See License.txt in the project root for license information.
// </license>
//-----------------------------------------------------------------------
#endregion

namespace Framework.Base
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceProcess;

    /// <summary>
    /// Holds all the configuration settings.
    /// </summary>
    [DataContract, Serializable]
    public class ConfigurationStorage
    {
        /// <summary>
        /// Gets or sets a value indicating whether logging should be verbose.
        /// </summary>
        /// <value>
        ///   <c>true</c> if logging should be verbose; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool LoggingVerbose { get; set; }

        /// <summary>
        /// Gets or sets the logging pattern.
        /// </summary>
        /// <value>
        /// The logging pattern.
        /// </value>
        [DataMember]
        public LoggingPattern LoggingPattern { get; set; }

        /// <summary>
        /// Gets or sets the receiving pattern for the event hubs.
        /// </summary>
        /// <value>
        /// The receiving pattern for the event hubs.
        /// </value>
        [DataMember]
        public EHReceivePatternType EventHubsReceivingPattern { get; set; }

        /// <summary>
        /// Gets or sets the starting application default type.
        /// </summary>
        /// <value>
        /// The starting application default type.
        /// </value>
        [DataMember]
        public string DefaultStartingApplicationType { get; set; }

        /// <summary>
        /// Gets or sets the type of the Windows NT service account.
        /// </summary>
        /// <value>
        /// The type of the Windows NT service account.
        /// </value>
        [DataMember]
        public ServiceAccount WindowsNTServiceAccountType { get; set; }

        /// <summary>
        /// Gets or sets the Windows NT service password.
        /// </summary>
        /// <value>
        /// The Windows NT service password.
        /// </value>
        [DataMember]
        public string WindowsNTServicePassword { get; set; }

        /// <summary>
        /// Gets or sets the Windows NT service user name.
        /// </summary>
        /// <value>
        /// The Windows NT service user name.
        /// </value>
        [DataMember]
        public string WindowsNTServiceUsername { get; set; }

        /// <summary>
        /// Gets or sets the name of the Windows NT service.
        /// </summary>
        /// <value>
        /// The name of the Windows NT service.
        /// </value>
        [DataMember]
        public string WindowsNTServiceName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the Windows NT service.
        /// </summary>
        /// <value>
        /// The display name of the Windows NT service.
        /// </value>
        [DataMember]
        public string WindowsNTServiceDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the Windows NT service description.
        /// </summary>
        /// <value>
        /// The Windows NT service description.
        /// </value>
        [DataMember]
        public string WindowsNTServiceDescription { get; set; }

        /// <summary>
        /// Gets or sets the wait time before restarting.
        /// </summary>
        /// <value>
        /// The wait time before restarting.
        /// </value>
        [DataMember]
        public int WaitTimeBeforeRestarting { get; set; }

        /// <summary>
        /// Gets or sets the web API end point.
        /// </summary>
        /// <value>
        /// The web API end point.
        /// </value>
        [DataMember]
        public string WebApiEndPoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable persisting messaging.
        /// </summary>
        /// <value>
        /// <c>true</c> if persisting messaging should be enabled; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool EnablePersistingMessaging { get; set; }

        /// <summary>
        /// Gets or sets the directory service executable.
        /// </summary>
        /// <value>
        /// The directory service executable.
        /// </value>
        public string DirectoryServiceExecutable { get; set; }

        /// <summary>
        /// Gets or sets the name of the directory operative root executable.
        /// </summary>
        /// <value>
        /// The name of the directory operative root executable.
        /// </value>
        public string DirectoryOperativeRootExeName { get; set; }

        /// <summary>
        /// Gets or sets the connection string for the receive group event hubs.
        /// </summary>
        /// <value>
        /// The connection string for the receive group event hubs.
        /// </value>
        [DataMember]
        public string GroupEventHubsReceiveConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the connection string for the send group event hubs.
        /// </summary>
        /// <value>
        /// The connection string for the send group event hubs.
        /// </value>
        [DataMember]
        public string GroupEventHubsSendConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the group event hubs.
        /// </summary>
        /// <value>
        /// The name of the group event hubs.
        /// </value>
        [DataMember]
        public string GroupEventHubsName { get; set; }

        /// <summary>
        /// Gets or sets the storage account name used by the group event hubs.
        /// </summary>
        /// <value>
        /// The storage account name used by the group event hubs.
        /// </value>
        [DataMember]
        public string GroupEventHubsStorageAccountName { get; set; }

        /// <summary>
        /// Gets or sets the storage account key used by group event hubs.
        /// </summary>
        /// <value>
        /// The storage account key used by group event hubs.
        /// </value>
        [DataMember]
        public string GroupEventHubsStorageAccountKey { get; set; }

        /// <summary>
        /// Gets or sets the connection string for the log receive group event hubs.
        /// </summary>
        /// <value>
        /// The connection string for the log receive group event hubs.
        /// </value>
        [DataMember]
        public string GroupEventHubsLogReceiveConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the connection string for the log send group event hubs.
        /// </summary>
        /// <value>
        /// The connection string for the log send group event hubs.
        /// </value>
        [DataMember]
        public string GroupEventHubsLogSendConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the log group event hubs.
        /// </summary>
        /// <value>
        /// The name of the log group event hubs.
        /// </value>
        [DataMember]
        public string GroupEventHubsLogName { get; set; }

        /// <summary>
        /// Gets or sets the storage account name of the log group event hubs.
        /// </summary>
        /// <value>
        /// The storage account name of the log group event hubs.
        /// </value>
        [DataMember]
        public string GroupEventHubsLogStorageAccountName { get; set; }

        /// <summary>
        /// Gets or sets the storage account key of the log group event hubs.
        /// </summary>
        /// <value>
        /// The storage account key of the log group event hubs.
        /// </value>
        [DataMember]
        public string GroupEventHubsLogStorageAccountKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the azure namespace.
        /// </summary>
        /// <value>
        /// The name of the azure namespace.
        /// </value>
        [DataMember]
        public string GroupAzureNamespaceName { get; set; }

        /// <summary>
        /// Gets or sets the local storage provider.
        /// </summary>
        /// <value>
        /// The local storage provider.
        /// </value>
        [DataMember]
        public string GetLocalStorageProvider { get; set; }

        /// <summary>
        /// Gets or sets the local storage connection string.
        /// </summary>
        /// <value>
        /// The local storage connection string.
        /// </value>
        [DataMember]
        public string GetLocalStorageConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the point name of the jitGate.
        /// </summary>
        /// <value>
        /// The point name of the jitGate.
        /// </value>
        [DataMember]
        public string jitGatePointName { get; set; }

        /// <summary>
        /// Gets or sets the point identifier for jitGate.
        /// </summary>
        /// <value>
        /// The point identifier for jitGate.
        /// </value>
        [DataMember]
        public string jitGatePointID { get; set; }

        /// <summary>
        /// Gets or sets the point description for jitGate.
        /// </summary>
        /// <value>
        /// The point description for jitGate.
        /// </value>
        [DataMember]
        public string jitGatePointDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether state should be logged.
        /// </summary>
        /// <value>
        ///   <c>true</c> if state should be logged; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool LoggingStateEnabled { get; set; }

        /// <summary>
        /// Gets or sets the global polling time in milliseconds.
        /// </summary>
        /// <value>
        /// The global polling time in milliseconds.
        /// </value>
        [DataMember]
        public int GlobalPollingTime { get; set; }

        /// <summary>
        /// Gets or sets the trigger polling sync time in milliseconds.
        /// </summary>
        /// <value>
        /// The trigger polling sync time in milliseconds
        /// </value>
        [DataMember]
        public int GlobalSyncTime { get; set; }

        /// <summary>
        /// Gets or sets the starting date time receiving from event hubs.
        /// </summary>
        /// <value>
        /// The starting date time receiving from event hubs.
        /// </value>
        [DataMember]
        public string EventHubsStartingDateTimeReceiving { get; set; }

        /// <summary>
        ///     Define the Event Hubs checkpoint pattern
        ///     Values(CheckPoint [use checpoint mechanism], DT [use datetime], DTE [use datetime epoch] )
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// Gets or sets the event hubs epoch.
        /// </summary>
        /// <value>
        /// The event hubs epoch.
        /// </value>
        [DataMember]
        public long EventHubsEpoch { get; set; }

        /// <summary>
        /// Gets or sets the check point pattern for event hubs.
        /// </summary>
        /// <value>
        /// The check point pattern for event hubs.
        /// </value>
        [DataMember]
        public EventHubsCheckPointPattern EventHubsCheckPointPattern { get; set; }
    } // ConfigurationStorage
} // namespace