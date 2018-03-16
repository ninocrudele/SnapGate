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

using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

#endregion

namespace SnapGate.Framework.Engine
{
    /// <summary>
    ///     The RestEventsEngine interface.
    /// </summary>
    [ServiceContract]
    public interface IRestEventsEngine
    {
        /// <summary>
        ///     The sync send bubbling configuration.
        /// </summary>
        /// <param name="channelId">
        ///     The channel id.
        /// </param>
        /// <param name="pointId">
        ///     The point id.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [OperationContract]
        [WebGet]
        string Deploy(string configuration, string platform);

        [OperationContract]
        [WebGet]
        string SyncPush(string channelId, string pointId);

        [OperationContract]
        [WebGet]
        string SyncPull(string channelId, string pointId);

        [OperationContract]
        [WebGet]
        string Sync();

        /// <summary>
        ///     The configuration.
        /// </summary>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        [OperationContract]
        [WebGet]
        Stream Configuration();

        /// <summary>
        ///     The execute trigger.
        /// </summary>
        /// <param name="triggerId">
        ///     The trigger id.
        /// </param>
        /// <param name="configurationId"></param>
        /// <param name="value"></param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [OperationContract]
        [WebGet]
        string ExecuteTrigger(string configurationId, string triggerId, string value);

        /// <summary>
        ///     The refresh bubbling setting.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        [OperationContract]
        [WebGet]
        string RefreshBubblingSetting();
    }
}