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

using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace SnapGateTrigger.Trigger
{
    //<USING>
    /// <summary>
    ///     The nop trigger.
    /// </summary>
    [TriggerContract("{*ID*}", "*NAME*", "*DESCRIPTION*", false, true, false)]
    public class SnapGateTrigger : ITriggerType
    {
        //<PROPERTIES>
        [TriggerPropertyContract("Syncronous",
            "Define if the action between the trigger and the remote event needs to be syncronous")]
        public bool Syncronous { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action trigger.
        /// </summary>
        public ActionTrigger ActionTrigger { get; set; }

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        [TriggerPropertyContract("DataContext", "Main data context")]
        public byte[] DataContext { get; set; }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="actionTrigger">
        ///     The set event action trigger.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        [TriggerActionContract("{*CONTRACTID*}", "Main action", "Main action executed by the trigger")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            //<MAINCODE>

            actionTrigger(this, context);
            return null;
        }

        //<FUNCTIONS>
    }
}