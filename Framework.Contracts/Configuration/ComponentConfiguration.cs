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

#endregion

namespace SnapGate.Framework.Contracts.Configuration
{
    /// <summary>
    ///     Component event File
    /// </summary>
    [DataContract]
    [Serializable]
    public class ComponentConfiguration
    {
        /// <summary>
        ///     Gets or sets the event.
        /// </summary>
        [DataMember]
        public Component Component { get; set; }
    }

    /// <summary>
    ///     The component.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Component
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
        public Component(string idComponent, string name, string description)
        {
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
        ///     Gets or sets the event properties.
        /// </summary>
        [DataMember]
        public List<ComponentProperty> ComponentProperties { get; set; }
    }

    /// <summary>
    ///     The event property.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ComponentProperty
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
        public ComponentProperty(string name, object value)
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
}