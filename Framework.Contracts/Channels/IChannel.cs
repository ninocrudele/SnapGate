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
using SnapGate.Framework.Contracts.Points;

#endregion

namespace SnapGate.Framework.Contracts.Channels
{
    /// <summary>
    ///     The Channel interface.
    /// </summary>
    internal interface IChannel
    {
        /// <summary>
        ///     Gets or sets the channel id.
        /// </summary>
        string ChannelId { get; set; }

        /// <summary>
        ///     Gets or sets the channel name.
        /// </summary>
        string ChannelName { get; set; }

        /// <summary>
        ///     Gets or sets the channel description.
        /// </summary>
        string ChannelDescription { get; set; }

        /// <summary>
        ///     Gets or sets the points.
        /// </summary>
        List<Point> Points { get; set; }
    }
}