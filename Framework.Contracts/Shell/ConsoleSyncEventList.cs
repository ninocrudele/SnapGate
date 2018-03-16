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
using System.Runtime.Serialization;
using SnapGate.Framework.Contracts.Configuration;
using SnapGate.Framework.Contracts.Points;

#endregion

namespace SnapGate.Framework.Contracts.Shell
{
    /// <summary>
    ///     The console sync event list.
    /// </summary>
    [DataContract]
    public class ConsoleSyncEventList
    {
        /// <summary>
        ///     Gets or sets the point.
        /// </summary>
        [DataMember]
        public Point Point { get; set; }

        /// <summary>
        ///     Gets or sets the trigger configuration list.
        /// </summary>
        [DataMember]
        public List<TriggerConfiguration> TriggerConfigurationList { get; set; }

        /// <summary>
        ///     Gets or sets the event configuration list.
        /// </summary>
        [DataMember]
        public List<EventConfiguration> EventConfigurationList { get; set; }
    }
}