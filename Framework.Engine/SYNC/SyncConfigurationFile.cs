﻿// Copyright 2016-2040 Nino Crudele
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

namespace SnapGate.Framework.Engine
{
    /// <summary>
    ///     The sync configuration file.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SyncConfigurationFile
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SyncConfigurationFile" /> class.
        /// </summary>
        /// <param name="fileType">
        ///     The file type.
        /// </param>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="fileContent">
        ///     The file content.
        /// </param>
        /// <param name="channelId">
        ///     The channel id.
        /// </param>
        public SyncConfigurationFile(
            string fileType,
            string name,
            byte[] fileContent,
            string channelId)
        {
            FileType = fileType;
            Name = name;
            FileContent = fileContent;
            ChannelId = channelId;
        }

        /// <summary>
        ///     Gets or sets the file type.
        /// </summary>
        [DataMember]
        public string FileType { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the file content.
        /// </summary>
        [DataMember]
        public byte[] FileContent { get; set; }

        /// <summary>
        ///     Gets or sets the channel id.
        /// </summary>
        [DataMember]
        public string ChannelId { get; set; }
    }
}