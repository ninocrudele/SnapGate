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
using SnapGate.Framework.Contracts.Channels;

#endregion

namespace SnapGate.Framework.Contracts.Configuration
{
    /// <summary>
    ///     Trigger configuration File, To create a configuration file trigger who able to activate a trigger action/s
    /// </summary>
    [DataContract]
    [Serializable]
    public class TriggerConfiguration
    {
        /// <summary>
        ///     Gets or sets the trigger.
        /// </summary>
        [DataMember]
        public Trigger Trigger { get; set; }

        /// <summary>
        ///     Gets or sets the events.
        /// </summary>
        [DataMember]
        public List<Event> Events { get; set; }
    }

    /// <summary>
    ///     The trigger.
    /// </summary>
    [DataContract, Serializable]
    public class Trigger
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Trigger" /> class.
        /// </summary>
        /// <param name="idComponent">
        ///     The id component.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public Trigger(string idComponent, string idConfiguration, string name, string description)
        {
            IdComponent = idComponent;
            IdConfiguration = idConfiguration;
            Name = name;
            Description = description;
        }


        /// <summary>
        ///     Gets or sets the id component.
        /// </summary>
        [DataMember]
        public string IdComponent { get; set; }

        /// <summary>
        ///     Get or set the id configuration
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
        ///     Gets or sets the channels where the trigger could be executes, if null then can be executed anyway.
        /// </summary>
        [DataMember]
        public List<Channel> Channels { get; set; }

        /// <summary>
        ///     Gets or sets the id chains.
        /// </summary>
        [DataMember]
        public List<Chain> Chains { get; set; }

        /// <summary>
        ///     Gets or sets the trigger properties.
        /// </summary>
        [DataMember]
        public List<TriggerProperty> TriggerProperties { get; set; }
    }

    /// <summary>
    ///     The trigger property.
    /// </summary>
    [DataContract, Serializable]
    public class TriggerProperty
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TriggerProperty" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public TriggerProperty(string name, object value)
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
    ///     The trigger action.
    /// </summary>
    [DataContract, Serializable]
    public class TriggerAction
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TriggerAction" /> class.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        public TriggerAction(string id, string name)
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