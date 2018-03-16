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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using SnapGate.Framework.Contracts.Points;

#endregion

namespace SnapGate.Framework.Contracts.Shell
{
    /// <summary>
    ///     The shell item type.
    /// </summary>
    public enum ShellItemType
    {
        /// <summary>
        ///     The host.
        /// </summary>
        Host,

        /// <summary>
        ///     The point.
        /// </summary>
        Point,

        /// <summary>
        ///     The trigger dll.
        /// </summary>
        TriggerDll,

        /// <summary>
        ///     The event dll.
        /// </summary>
        EventDll,

        /// <summary>
        ///     The trigger configuration.
        /// </summary>
        TriggerConfiguration,

        /// <summary>
        ///     The event configuration.
        /// </summary>
        EventConfiguration
    }

    /// <summary>
    ///     The sync point message.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SyncPointMessage
    {
        /// <summary>
        ///     Gets or sets the host.
        /// </summary>
        [DataMember]
        public Host Host { get; set; }

        /// <summary>
        ///     Gets or sets the point.
        /// </summary>
        [DataMember]
        public Point Point { get; set; }

        /// <summary>
        ///     Gets or sets the bubbling triggers.
        /// </summary>
        [DataMember]
        public List<SyncPointFile> BubblingTriggers { get; set; }

        /// <summary>
        ///     Gets or sets the bubbling events.
        /// </summary>
        [DataMember]
        public List<SyncPointFile> BubblingEvents { get; set; }

        /// <summary>
        ///     Gets or sets the triggers.
        /// </summary>
        [DataMember]
        public List<SyncPointFile> DllTriggers { get; set; }

        /// <summary>
        ///     Gets or sets the events.
        /// </summary>
        [DataMember]
        public List<SyncPointFile> DllEvents { get; set; }
    }

    /// <summary>
    ///     The host.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Host
    {
        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the ip.
        /// </summary>
        [DataMember]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public string Ip { get; set; }

        /// <summary>
        ///     Gets or sets the description.
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}