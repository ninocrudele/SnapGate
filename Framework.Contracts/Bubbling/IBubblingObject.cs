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

using System.Collections.Generic;
using SnapGate.Framework.Contracts.Configuration;

#endregion

namespace SnapGate.Framework.Contracts.Bubbling
{
    /// <summary>
    ///     The BubblingEvent interface.
    /// </summary>
    public interface IBubblingObject
    {
        byte[] Data { get; set; }

        /// <summary>
        ///     Trigger or Event
        /// </summary>
        BubblingEventType BubblingEventType { get; set; }

        /// <summary>
        ///     If trigger type and a file trigger event is present in the Bubbling directory this prop is true
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        ///     Endpoints destinations
        /// </summary>
        string IdComponent { get; set; }

        /// <summary>
        ///     Gets or sets the id configuration.
        /// </summary>
        string IdConfiguration { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether shared.
        /// </summary>
        bool Shared { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether polling required.
        /// </summary>
        bool PollingRequired { get; set; }

        /// <summary>
        ///     Gets or sets the properties.
        /// </summary>
        Dictionary<string, Property> Properties { get; set; }

        /// <summary>
        ///     Gets or sets the properties.
        /// </summary>
        IDictionary<string, string> PropertiesContext { get; set; }


        /// <summary>
        ///     Gets or sets the events.
        /// </summary>
        List<Event> Events { get; set; }

        /// <summary>
        ///     Used to transport the raw data betwwen the source and the destination point
        ///     Just the raw data picked up by the trigger or event
        /// </summary>
        byte[] DataContext { get; set; }

        //To manage the Sync Async behaviour
        /// <summary>
        ///     Used to notify that the request/response is syncronous
        /// </summary>
        bool Syncronous { get; set; }

        /// <summary>
        ///     Token used to identify the delegate to search and execute when back to the source
        /// </summary>
        string SyncronousToken { get; set; }

        /// <summary>
        ///     If it's a response syncronous from event
        /// </summary>
        bool SyncronousFromEvent { get; set; }

        string SubscriberId { get; set; }

        string ByteArray { get; set; }

        string DestinationChannelId { get; set; }

        string DestinationPointId { get; set; }

        string SenderChannelId { get; set; }

        string SenderChannelName { get; set; }

        string SenderChannelDescription { get; set; }

        string SenderPointId { get; set; }

        string SenderName { get; set; }

        string SenderDescriprion { get; set; }

        string Embedded { get; set; }

        string MessageType { get; set; }

        string MessageId { get; set; }

        bool Persisting { get; set; }

        string Event { get; set; }

        string Trigger { get; set; }

        string SyncSendLocalDll { get; set; }

        string SyncSendBubblingConfiguration { get; set; }
        //Send the bubbling configuration  {get; set;} the receiver will put in gcpoints

        string SyncSendRequestBubblingConfiguration { get; set; }

        string SyncSendFileBubblingConfiguration { get; set; }

        string SyncSendRequestConfiguration { get; set; } //Send the request for all the configuration 

        string SyncSendConfiguration { get; set; } //Send the request for all the configuration 

        string SyncSendRequestComponent { get; set; } //Send a request to receive a component to sync

        string TriggerEventJson { get; set; }

        string EventPropertyJson { get; set; }

        string SyncRequestConfirmed { get; set; }

        string SyncAvailable { get; set; }

        string ConsoleRequestSendBubblingBag { get; set; }

        string ConsoleBubblingBagToSyncronize { get; set; }
    }
}