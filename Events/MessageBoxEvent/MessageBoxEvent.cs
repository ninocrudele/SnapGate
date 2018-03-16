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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;

#endregion

namespace SnapGate.Framework.MessageBoxEvent
{
    /// <summary>
    ///     The message box event.
    /// </summary>
    [EventContract("{DC78440A-E82B-4A5C-B4C5-C78CC40472BD}", "MessageBox Event", "Show a messagebox", true)]
    public class MessageBoxEvent : IEventType
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
        [EventActionContract("{1B28FEDB-3940-463D-BA80-223B4C40FE31}", "Main action", "Main action description")]
        public async Task Execute(ActionEvent actionEvent, ActionContext context)
        {
            try
            {
                Trace.WriteLine("In MessageBoxEvent Event.");
                var message = EncodingDecoding.EncodingBytes2String(DataContext);
                MessageBox.Show(message);
                actionEvent(this, context);
                return;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("MessageBoxEvent error > " + ex.Message);
                actionEvent(this, null);
                return;
            }
        }
    }
}