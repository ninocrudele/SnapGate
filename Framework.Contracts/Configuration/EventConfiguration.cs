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
using System.Runtime.Serialization;
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Channels;

#endregion

namespace SnapGate.Framework.Contracts.Configuration
{
    /// <summary>
    ///     Trigger event File
    /// </summary>
    [DataContract]
    [Serializable]
    public class EventConfiguration
    {
        /// <summary>
        ///     Gets or sets the event.
        /// </summary>
        [DataMember]
        public Event Event { get; set; }
    }

    /// <summary>
    ///     The event.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Event
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Event" /> class.
        /// </summary>
        /// <param name="idComponent">
        ///     The id component.
        /// </param>
        /// <param name="idConfiguration">
        ///     The id configuration.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public Event(string idComponent, string idConfiguration, string name, string description)
        {
            IdConfiguration = idConfiguration;
            IdComponent = idComponent;
            Name = name;
            Description = description;
        }

        /// <summary>
        ///     Gets or sets the id component.
        /// </summary>
        [DataMember]
        public string IdComponent { get; set; }

        /// <summary>
        ///     Gets or sets the id configuration.
        /// </summary>
        [DataMember]
        public string IdConfiguration { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the id chains.
        /// </summary>
        [DataMember]
        public List<Chain> Chains { get; set; }

        /// <summary>
        ///     Gets or sets the event properties.
        /// </summary>
        [DataMember]
        public List<EventProperty> EventProperties { get; set; }

        /// <summary>
        ///     Cache in dictionary the event properties.
        /// </summary>
        public Dictionary<string, EventProperty> CacheEventProperties { get; set; }

        /// <summary>
        ///     Gets or sets the channels.
        /// </summary>
        [DataMember]
        public List<Channel> Channels { get; set; }

        /// <summary>
        ///     Gets or sets the correlation.
        /// </summary>
        [DataMember]
        public Correlation Correlation { get; set; }
    }

    /// <summary>
    ///     The event property.
    /// </summary>
    [DataContract]
    [Serializable]
    public class EventProperty
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventProperty" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public EventProperty(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     Property name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        [DataMember]
        public object Value { get; set; }
    }

    /// <summary>
    ///     The event action.
    /// </summary>
    [DataContract]
    [Serializable]
    public class EventAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventAction" /> class.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        public EventAction(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        ///     Unique Action ID
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        ///     Method name
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}