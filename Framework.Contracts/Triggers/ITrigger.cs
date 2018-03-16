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

using SnapGate.Framework.Contracts.Globals;

#endregion

namespace SnapGate.Framework.Contracts.Triggers
{
    /// <summary>
    ///     The TriggerType interface.
    /// </summary>
    public interface ITriggerType
    {
        /// <summary>
        ///     Internal Trigger context.
        /// </summary>
        ActionContext Context { get; set; }

        /// <summary>
        ///     internal delegate to use in delegates events
        /// </summary>
        ActionTrigger ActionTrigger { get; set; }

        /// <summary>
        ///     Identify if the trigger request must be syncronous or not
        /// </summary>
        bool Syncronous { get; set; }

        /// <summary>
        ///     Main default data property
        /// </summary>
        byte[] DataContext { get; set; }

        /// <summary>
        ///     Main default method
        /// </summary>
        /// <param name="actionTrigger">
        ///     The set Event Action Trigger.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        byte[] Execute(ActionTrigger actionTrigger, ActionContext context);
    }
}