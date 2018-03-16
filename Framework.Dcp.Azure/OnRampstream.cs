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
using System.Reflection;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Messaging;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.Dcp.Azure
{
    /// <summary>
    ///     Main Downstream events receiving
    ///     It execute the main DownStream Instance
    /// </summary>
    [EventsOnRampContract("{B8ECF14B-2A9E-41C9-9E85-D8EA2D5C4E22}", "EventsDownStream", "Event Hubs EventsDownStream")]
    public class OnRampStream : IOnRampStream
    {
        private static SetEventOnRampMessageReceived SetEventOnRampMessageReceived { get; set; }


        public void Run(SetEventOnRampMessageReceived setEventOnRampMessageReceived)
        {
            try
            {
                // Assign the delegate 
                SetEventOnRampMessageReceived = setEventOnRampMessageReceived;
                // Load vars
                var eventHubConnectionString = ConfigurationBag.Configuration.AzureNameSpaceConnectionString;
                var eventHubName = ConfigurationBag.Configuration.GroupEventHubsName;

                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Event Hubs transfort Type: {ConfigurationBag.Configuration.ServiceBusConnectivityMode}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    null,
                    Constant.LogLevelInformation);

                var builder = new ServiceBusConnectionStringBuilder(eventHubConnectionString)
                {
                    TransportType =
                        TransportType.Amqp
                };

                //If not exit it create one, drop brachets because Azure rules
                var eventHubConsumerGroup =
                    string.Concat(ConfigurationBag.EngineName, "_", ConfigurationBag.Configuration.ChannelId)
                        .Replace("{", "")
                        .Replace("}", "")
                        .Replace("-", "");
                var nsManager = NamespaceManager.CreateFromConnectionString(builder.ToString());
                Trace.WriteLine($"Initializing Group Name {eventHubConsumerGroup}");

                Trace.WriteLine("Start DirectRegisterEventReceiving.");

                // Create Event Hubs
                var eventHubClient = EventHubClient.CreateFromConnectionString(builder.ToString(), eventHubName);
                // Create consumer
                nsManager.CreateConsumerGroupIfNotExists(eventHubName, eventHubConsumerGroup);

                var namespaceManager = NamespaceManager.CreateFromConnectionString(builder.ToString());
                var ehDescription = namespaceManager.GetEventHub(eventHubName);
                // Use the default consumer group

                foreach (var partitionId in ehDescription.PartitionIds)
                {
                    var myNewThread =
                        new Thread(() =>
                            ReceiveDirectFromPartition(eventHubClient, partitionId, eventHubConsumerGroup));
                    myNewThread.Start();
                }

                Trace.WriteLine(
                    "After DirectRegisterEventReceiving Downstream running.");
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name} - Hint: Check if the firewall outbound port 5671 is opened.",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesEventHubs,
                    ex,
                    Constant.LogLevelError);
            }
        }

        private static void ReceiveDirectFromPartition(
            EventHubClient eventHubClient,
            string partitionId,
            string consumerGroup)
        {
            try
            {
                var group = eventHubClient.GetConsumerGroup(consumerGroup);

                var eventHubsStartingDateTimeReceiving =
                    DateTime.Parse(
                        ConfigurationBag.Configuration.EventHubsStartingDateTimeReceiving == "0"
                            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                            ? DateTime.UtcNow.ToString()
                            : ConfigurationBag.Configuration.EventHubsStartingDateTimeReceiving);
                var eventHubsEpoch = ConfigurationBag.Configuration.EventHubsEpoch;

                EventHubReceiver receiver = null;

                switch (ConfigurationBag.Configuration.EventHubsCheckPointPattern)
                {
                    case EventHubsCheckPointPattern.CheckPoint:
                        //Receiving from the last valid receiving point
                        receiver = group.CreateReceiver(partitionId, DateTime.UtcNow);
                        break;
                    case EventHubsCheckPointPattern.Dt:

                        receiver = group.CreateReceiver(partitionId, eventHubsStartingDateTimeReceiving);
                        break;
                    case EventHubsCheckPointPattern.Dtepoch:
                        receiver = group.CreateReceiver(partitionId, eventHubsStartingDateTimeReceiving,
                            eventHubsEpoch);
                        break;
                    case EventHubsCheckPointPattern.Dtutcnow:
                        receiver = group.CreateReceiver(partitionId, DateTime.UtcNow);
                        break;
                    case EventHubsCheckPointPattern.Dtnow:
                        receiver = group.CreateReceiver(partitionId, DateTime.Now);
                        break;
                    case EventHubsCheckPointPattern.Dtutcnowepoch:
                        receiver = group.CreateReceiver(partitionId, DateTime.UtcNow, eventHubsEpoch);
                        break;
                    case EventHubsCheckPointPattern.Dtnowepoch:
                        receiver = group.CreateReceiver(partitionId, DateTime.Now, eventHubsEpoch);
                        break;
                }

                Trace.WriteLine($"Direct Receiver created. Partition {partitionId}");
                while (true)
                {
                    var message = receiver?.Receive();
                    if (message != null)
                    {
                        BubblingObject bubblingObject = BubblingObject.DeserializeMessage(message.GetBytes());
                        SetEventOnRampMessageReceived(bubblingObject);
                    }
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