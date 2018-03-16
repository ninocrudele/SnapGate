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

using System.Threading.Tasks;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;

#endregion

namespace SnapGate.Framework.NopEvent
{
    /// <summary>
    ///     The no operation event.
    /// </summary>
    [EventContract("{D1EC2907-56A9-474B-B08A-750E72F0C29D}", "NOP Event",
        "No operation Event, used for testing purpose.", true)]
    public class NopEvent : IEventType
    {
        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action event.
        /// </summary>
        public ActionEvent ActionEvent { get; set; }

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        [EventPropertyContract("DataContext", "Event Default Main Data")]
        public byte[] DataContext { get; set; }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="actionEvent">
        ///     The set event action event.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        [EventActionContract("{5A42100D-7D63-43AA-B141-C658F5BFFB59}", "Main action", "Main action description")]
        public async Task Execute(ActionEvent actionEvent, ActionContext context)
        {
            return;
        }
    }
}