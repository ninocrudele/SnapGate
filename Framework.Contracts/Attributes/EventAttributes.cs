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

#endregion

namespace SnapGate.Framework.Contracts.Attributes
{
    /// <summary>
    ///     The event contract.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class EventContract : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventContract" /> class.
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
        /// <param name="shared">
        ///     The shared.
        /// </param>
        public EventContract(string id, string name, string description, bool shared)
        {
            Id = id;
            Name = name;
            Description = description;
            Shared = shared;
        }

        /// <summary>
        ///     Unique Action ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Method name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether shared.
        /// </summary>
        public bool Shared { get; set; }
    }

    /// <summary>
    ///     The event property contract.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EventPropertyContract : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventPropertyContract" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        public EventPropertyContract(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        ///     Method name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    ///     The event action contract.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EventActionContract : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventActionContract" /> class.
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
        public EventActionContract(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        /// <summary>
        ///     Unique Action ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Method name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
    }
}