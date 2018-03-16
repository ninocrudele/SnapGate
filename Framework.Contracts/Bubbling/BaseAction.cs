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
    public class BaseAction : IAction
    {
        public BaseAction(string ID, string Name, string Description, MethodInfo AssemblyMethodInfo, string ReturnValue)
        {
            Id = ID;
            this.Name = Name;
            this.Description = Description;
            this.AssemblyMethodInfo = AssemblyMethodInfo;
            this.ReturnValue = ReturnValue;
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
        ///     Internal Method to invoke
        /// </summary>
        public MethodInfo AssemblyMethodInfo { get; set; }

        /// <summary>
        ///     Description
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        ///     Property Value
        /// </summary>
        [DataMember]
        public object ReturnValue { get; set; }

        /// <summary>
        ///     Bubbling parameters
        /// </summary>
        [DataMember]
        public List<Parameter> Parameters { get; set; }
    }
}