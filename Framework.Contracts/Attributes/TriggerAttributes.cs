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
using System.Runtime.Serialization;

#endregion

namespace SnapGate.Framework.Contracts.Attributes
{
    /// <summary>
    ///     The trigger contract.
    /// </summary>
    [DataContract]
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)] // Multiuse attribute.
    public class TriggerContract : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TriggerContract" /> class.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        /// <param name="pollingRequired">
        ///     The polling required.
        /// </param>
        /// <param name="shared">
        ///     The shared.
        /// </param>
        /// <param name="nop">
        ///     The no operation.
        /// </param>
        public TriggerContract(string id, string name, string description, bool pollingRequired, bool shared, bool nop)
        {
            Id = id;
            Name = name;
            Description = description;
            Shared = shared;
            PollingRequired = pollingRequired;
            Nop = nop;
        }

        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

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
        ///     Gets or sets a value indicating whether shared.
        /// </summary>
        [DataMember]
        public bool Shared { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether polling required.
        /// </summary>
        [DataMember]
        public bool PollingRequired { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether no operation.
        /// </summary>
        [DataMember]
        public bool Nop { get; set; }
    }

    /// <summary>
    ///     The trigger property contract.
    /// </summary>
    [DataContract]
    [Serializable]
    [AttributeUsage(AttributeTargets.Property)]
    public class TriggerPropertyContract : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TriggerPropertyContract" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public TriggerPropertyContract(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        ///     Property name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }

    /// <summary>
    ///     The trigger action contract.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [DataContract]
    [Serializable]
    public class TriggerActionContract : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TriggerActionContract" /> class.
        /// </summary>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public TriggerActionContract(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
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

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}