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
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Log;

#endregion

namespace SnapGate.Framework.Log.EventHubs
{
    /// <summary>
    ///     Send messages to EH
    /// </summary>
    internal static class LogEventUpStream
    {
        //EH variable

        private static string azureNameSpaceConnectionString = "";

        private static string eventHubName = "";

        private static EventHubClient eventHubClient;

        public static bool CreateEventUpStream()
        {
            try
            {
                Trace.WriteLine("-------------- Engine LogEventUpStream --------------");
                Trace.WriteLine("LogEventUpStream - Get Configuration settings.");
                //Event Hub Configuration
                azureNameSpaceConnectionString = ConfigurationBag.Configuration.AzureNameSpaceConnectionString;
                eventHubName = ConfigurationBag.Configuration.LoggingComponentStorage;
                Trace.WriteLine($"LogEventUpStream - azureNameSpaceConnectionString={azureNameSpaceConnectionString}");
                Trace.WriteLine($"LogEventUpStream - eventHubName={eventHubName}");

                // TODO ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Https;

                var builder = new ServiceBusConnectionStringBuilder(azureNameSpaceConnectionString)
                {
                    TransportType =
                        TransportType
                            .Amqp
                };

                Trace.WriteLine("LogEventUpStream - Create the eventHubClient.");

                eventHubClient = EventHubClient.CreateFromConnectionString(builder.ToString(), eventHubName);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("LogEventUpStream - error->{0}", ex.Message);
                EventLog.WriteEntry("Framework.Log.EventHubs", ex.Message);
                return false;
            }
        }

        /// <summary>
        ///     Send a EventMessage message
        /// </summary>
        public static bool SendMessage(LogMessage logMessage)
        {
            try
            {
                Trace.WriteLine("LogEventUpStream - serialize log message.");
                //Create EH data message
                var jsonSerialized = JsonConvert.SerializeObject(logMessage);
                var serializedMessage = EncodingDecoding.EncodingString2Bytes(jsonSerialized);

                var data = new EventData(serializedMessage);
                Trace.WriteLine("LogEventUpStream - send log message.");

                //Send the metric to Event Hub
                eventHubClient.Send(data);
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("LogEventUpStream - error->{0}", ex.Message);

                EventLog.WriteEntry("Framework.Log.EventHubs", ex.Message, EventLogEntryType.Error);
                return false;
            }
        }
    }
}