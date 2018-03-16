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
using System.Runtime.Serialization;

#endregion

namespace SnapGate.Framework.Contracts.Bubbling
{
    /// <summary>
    ///     The Lower receive layer, this receive the raw data
    /// </summary>
    [DataContract]
    [Serializable]
    public class Property : IProperty
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Property" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="description">
        ///     The description.
        /// </param>
        /// <param name="assemblyPropertyInfo">
        ///     The assembly property info.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public Property(string name, string description, PropertyInfo assemblyPropertyInfo, Type type, object value)
        {
            Name = name;
            Description = description;
            AssemblyPropertyInfo = assemblyPropertyInfo;
            Type = type;
            Value = value;
        }

        /// <summary>
        ///     Description of method
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        ///     Initialize the clone
        /// </summary>
        /// <returns></returns>
        /// <summary>
        ///     Property name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     PropertyInfo  to evaluate
        /// </summary>
        public PropertyInfo AssemblyPropertyInfo { get; set; }

        /// <summary>
        ///     Property Type
        /// </summary>
        [DataMember]
        public Type Type { get; set; }

        /// <summary>
        ///     Property Value
        /// </summary>
        [DataMember]
        public object Value { get; set; }
    }
}