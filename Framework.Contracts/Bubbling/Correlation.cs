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
using SnapGate.Framework.Contracts.Configuration;

#endregion

namespace SnapGate.Framework.Contracts.Bubbling
{
    /// <summary>
    ///     The correlation.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Correlation
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Correlation" /> class.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="scriptRule">
        ///     The script rule.
        /// </param>
        /// <param name="events">
        ///     The events.
        /// </param>
        public Correlation(string name, string scriptRule, List<Event> events)
        {
            Name = name;
            ScriptRule = scriptRule;
            Events = events;
        }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the script rule.
        /// </summary>
        [DataMember]
        public string ScriptRule { get; set; }

        /// <summary>
        ///     Gets or sets the events.
        /// </summary>
        [DataMember]
        public List<Event> Events { get; set; }
    }
}