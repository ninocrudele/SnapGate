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
using System.Threading.Tasks;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;

#endregion

namespace HM.OMS.PageOneMessageEvent
{
    /// <summary>
    ///     The file event.
    /// </summary>
    [EventContract("{BC76EEB8-369E-4B0B-BC52-4DFBD4FA33B1}", "PageOneMessageEvent",
        "PageOneMessageEvent is the OMS service to send mails", true)]
    public class PageOneMessageEvent : IEventType
    {
        /// <summary>
        ///     [message] to send an email - [auth] to check the authentication
        /// </summary>
        [EventPropertyContract("input", "[message] to send an email - [auth] to check the authentication")]
        public string input { get; set; }

        /// <summary>
        ///     [message] to send an email - [auth] to check the authentication
        /// </summary>
        [EventPropertyContract("From", "The mail from")]
        public string From { get; set; }

        /// <summary>
        ///     [message] to send an email - [auth] to check the authentication
        /// </summary>
        [EventPropertyContract("To", "The mail to")]
        public string To { get; set; }

        /// <summary>
        ///     [message] to send an email - [auth] to check the authentication
        /// </summary>
        [EventPropertyContract("Message", "Body message")]
        public string Message { get; set; }

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
        [EventActionContract("{E4FD267F-92B2-45F3-B198-0F1581E2EBBA}", "Main action", "Main action description")]
        public async Task Execute(ActionEvent actionEvent, ActionContext context)
        {
            try
            {
                DataContext = EncodingDecoding.EncodingString2Bytes(To);
                //Console.WriteLine("In event before handle");
                //Console.WriteLine($"EVENT {context.BubblingConfiguration.MessageId} - {DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");

                actionEvent(this, context);

                return;
            }
            catch (Exception ex)
            {
                DataContext = EncodingDecoding.EncodingString2Bytes(ex.Message);
                ;
                actionEvent(this, context);
                return;
            }
        }
    }
}