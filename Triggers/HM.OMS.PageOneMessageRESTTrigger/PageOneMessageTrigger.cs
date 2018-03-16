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
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Threading;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace HM.OMS.PageOneMessageRESTTrigger
{
    /// <summary>
    ///     The file trigger.
    /// </summary>
    [TriggerContract("{22F27D62-7D66-4947-9F08-D57E4E5FCC94}", "PageOneMessageTrigger",
        "PageOneMessageTrigger is the OMS service to send mails", false, true, false)]
    public class PageOneMessageTrigger : ITriggerType, IPageOneMessage
    {
        readonly static AutoResetEvent waitHandle = new AutoResetEvent(false);
        public static PageOneMessageTriggerWebServiceHost engineHost;

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

        /// <summary>
        ///     WebApiEndPoint used by the service
        /// </summary>
        [TriggerPropertyContract("WebApiEndPoint", "WebApiEndPoint used by the service")]
        public string WebApiEndPoint { get; set; }


        public string SupportBag { get; set; }


        public string SendMessage(iPageOneMessage pageOneMessage)
        {
            ActionTrigger(this, Context);
            //Wait for sync
            waitHandle.WaitOne();
            return EncodingDecoding.EncodingBytes2String(DataContext);
        }

        public bool ServiceAvailable()
        {
            return true;
        }

        public string Auth()
        {
            object s = engineHost;
            engineHost.ActionTrigger(engineHost.This, engineHost.Context);
            //Wait for sync
            waitHandle.WaitOne();
            return EncodingDecoding.EncodingBytes2String(engineHost.DataContext);
        }

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
        [TriggerActionContract("{4C60AEF9-7F37-456D-91E3-179E275F694B}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                //Start Service
                // Start Web API interface

                engineHost = new PageOneMessageTriggerWebServiceHost(typeof(PageOneMessageTrigger),
                    new Uri(WebApiEndPoint));
                engineHost.AddServiceEndpoint(typeof(IPageOneMessage), new WebHttpBinding(),
                    ConfigurationBag.EngineName);
                var stp = engineHost.Description.Behaviors.Find<ServiceDebugBehavior>();
                stp.HttpHelpPageEnabled = false;

                input = "/auth";
                engineHost.This = this;
                engineHost.Context = context;
                engineHost.ActionTrigger = actionTrigger;
                engineHost.Open();
                Thread.Sleep(Timeout.Infinite);
                return null;
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

    public class PageOneMessageTriggerWebServiceHost : WebServiceHost
    {
        public PageOneMessageTriggerWebServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public byte[] DataContext { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ITriggerType This { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action trigger.
        /// </summary>
        public ActionTrigger ActionTrigger { get; set; }
    }
}