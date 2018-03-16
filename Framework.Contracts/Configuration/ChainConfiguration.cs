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
    public class ChainConfiguration
    {
        /// <summary>
        ///     Gets or sets the event.
        /// </summary>
        [DataMember]
        public Chain Chain { get; set; }
    }

    /// <summary>
    ///     The component.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Chain
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Event" /> class.
        /// </summary>
        /// <param name="idChain">
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
        public Chain(string idChain, string name, string description)
        {
            IdChain = idChain;
            Name = name;
            Description = description;
        }

        /// <summary>
        ///     Gets or sets the id chain.
        /// </summary>
        [DataMember]
        public string IdChain { get; set; }

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
        public List<ComponentBag> Components { get; set; }
    }

    /// <summary>
    ///     The event property.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ComponentBag
    {
        public ComponentBag(string idcomponent)
        {
            idComponent = idcomponent;
        }

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        [DataMember]
        public string idComponent { get; set; }
    }
}