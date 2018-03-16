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

namespace SnapGate.Framework.Contracts.Bubbling
{
    /// <summary>
    ///     The assembly event.
    /// </summary>
    [DataContract]
    [Serializable]
    public class AssemblyEvent
    {
        /// <summary>
        ///     Gets or sets the id.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        ///     Gets or sets the version.
        /// </summary>
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        ///     Gets or sets the file name.
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        ///     Gets or sets the path file name.
        /// </summary>
        [DataMember]
        public string PathFileName { get; set; }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        ///     Gets or sets the assembly content.
        /// </summary>
        [DataMember]
        public byte[] AssemblyContent { get; set; }
    }
}