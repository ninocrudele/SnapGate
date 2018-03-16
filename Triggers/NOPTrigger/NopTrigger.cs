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

using System.Diagnostics.CodeAnalysis;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace SnapGate.Framework.NopTrigger
{
    /// <summary>
    ///     The nop trigger.
    /// </summary>
    [TriggerContract("{96860422-839B-40D0-8142-274EDF112F26}", "NOP Trigger",
        "No operation Trigger, used for test purpose.", false, true, false)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    public class NopTrigger : ITriggerType
    {
        public string SupportBag { get; set; }

        [TriggerPropertyContract("Syncronous", "Trigger Syncronous")]
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
        [TriggerPropertyContract("DataContext", "Trigger Default Main Data")]
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
        [TriggerActionContract("{4F767BB8-0071-444F-B4AD-ABF730F88B44}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            return null;
        }
    }
}