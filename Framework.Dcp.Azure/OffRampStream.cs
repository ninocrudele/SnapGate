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
using System.Reflection;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Messaging;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.Dcp.Azure
{
    /// <summary>
    ///     Send messages to EH
    /// </summary>
    [EventsOffRampContract("{6FAEA018-C21B-423E-B860-3F8BAC0BC637}", "EventUpStream", "Event Hubs EventUpStream")]
    public class OffRampStream : IOffRampStream
    {
        //EH variable

        private static string connectionString = "";

        private static string eventHubName = "";

        private static EventHubClient eventHubClient;

        public bool CreateOffRampStream()
        {
            try
            {
                //EH Configuration
                connectionString = ConfigurationBag.Configuration.AzureNameSpaceConnectionString;
                eventHubName = ConfigurationBag.Configuration.GroupEventHubsName;

                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Event Hubs transfort Type: {ConfigurationBag.Configuration.ServiceBusConnectivityMode}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    null,
                    Constant.LogLevelInformation);

                var builder = new ServiceBusConnectionStringBuilder(connectionString)
                {
                    TransportType =
                        TransportType.Amqp
                };

                eventHubClient = EventHubClient.CreateFromConnectionString(builder.ToString(), eventHubName);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Send a EventMessage message
        ///     invio importantissimo perche spedisce eventi e oggetti in array bytela dimensione e strategica
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(BubblingObject message)
        {
            try
            {
                byte[] byteArrayBytes = BubblingObject.SerializeMessage(message);
                EventData evtData = new EventData(byteArrayBytes);
                eventHubClient.Send(evtData);
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
    }
}