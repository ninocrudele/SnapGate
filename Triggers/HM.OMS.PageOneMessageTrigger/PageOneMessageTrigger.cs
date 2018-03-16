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
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace HM.OMS.PageOneMessageTrigger
{
    /// <summary>
    ///     The file trigger.
    /// </summary>
    [TriggerContract("{EFE0DDCC-4470-4D0C-AB00-BEB6549D8591}", "PageOneMessageTrigger",
        "PageOneMessageTrigger is the OMS service to send mails", false, true, false)]
    public class PageOneMessageTrigger : ITriggerType
    {
        /// <summary>
        ///     [message] to send an email - [auth] to check the authentication
        /// </summary>
        [TriggerPropertyContract("input", "[message] to send an email - [auth] to check the authentication")]
        public string input { get; set; }

        /// <summary>
        ///     [message] to send an email - [auth] to check the authentication
        /// </summary>
        [TriggerPropertyContract("From", "The mail from")]
        public string From { get; set; }

        /// <summary>
        ///     [message] to send an email - [auth] to check the authentication
        /// </summary>
        [TriggerPropertyContract("To", "The mail to")]
        public string To { get; set; }

        /// <summary>
        ///     [message] to send an email - [auth] to check the authentication
        /// </summary>
        [TriggerPropertyContract("Message", "Body message")]
        public string Message { get; set; }


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
        [TriggerActionContract("{D60E0168-8CDF-4443-B9AB-704E943012E3}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                if (input.Length != 0)
                {
                    actionTrigger(this, context);
                }

                return DataContext;
            }
            catch (Exception ex)
            {
                DataContext = EncodingDecoding.EncodingString2Bytes(ex.Message);
                actionTrigger(this, context);
                ActionTrigger = actionTrigger;
                Context = context;
                return EncodingDecoding.EncodingString2Bytes(ex.Message);
            }
        }
    }
}