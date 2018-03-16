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

using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace SnapGate.Framework.Contracts.Globals
{
    /// <summary>
    ///     Action that will be executed by the event in SyncAsync scenarios
    /// </summary>
    /// <param name="dataContext"></param>
    public delegate void SyncAsyncEventAction(byte[] dataContext);

    /// <summary>
    ///     Global Action Trigger Delegate used by triggers.
    /// </summary>
    /// <param name="_this">
    ///     The _this.
    /// </param>
    /// <param name="context">
    ///     The context.
    /// </param>
    public delegate void ActionTrigger(ITriggerType _this, ActionContext context);

    /// <summary>
    ///     Global Action Event Delegate used by events.
    /// </summary>
    /// <param name="_this">
    ///     The _this.
    /// </param>
    /// <param name="context">
    ///     The context.
    /// </param>
    public delegate void ActionEvent(IEventType _this, ActionContext context);

    /// <summary>
    ///     Global On Ramp Delegate used by on ramp receiver.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    public delegate void SetEventOnRampMessageReceived(BubblingObject message);

    /// <summary>
    ///     Global Off Ramp Delegate used by on ramp sender.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    public delegate void SetEventOnRampMessageSent(object message);


    /// <summary>
    ///     Context exchanged between triggers and events
    /// </summary>
    public class ActionContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActionContext" /> class.
        /// </summary>
        /// <param name="bubblingObjectBag">
        ///     The bubbling configuration.
        /// </param>
        public ActionContext(BubblingObject bubblingObjectBag)
        {
            BubblingObjectBag = bubblingObjectBag;
        }

        /// <summary>
        ///     Bag used to transport the triggers and events
        /// </summary>
        public BubblingObject BubblingObjectBag { get; set; }

        /// <summary>
        ///     MessageId sent across the points
        /// </summary>
        public string MessageId { get; set; }
    }
}