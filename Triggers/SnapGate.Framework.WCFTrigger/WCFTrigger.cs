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

using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

namespace SnapGate.Framework.WCFTrigger
{
    /// <summary>
    ///     The file trigger.
    /// </summary>
    [TriggerContract("{2D350D29-A626-4C67-BD60-6BE271B44E7C}", "WCFTrigger",
        "Create and host a generic WCF service.", false, true, false)]
    public class WCFTrigger : ITriggerType
    {
        /// <summary>
        ///     Gets or sets the regex file pattern.
        /// </summary>
        [TriggerPropertyContract("URIServiceEndpoint", "URI Service endpoint")]
        public string URIServiceEndpoint { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action trigger.
        /// </summary>
        public ActionTrigger ActionTrigger { get; set; }

        /// <summary>
        ///     If must be syncronous
        /// </summary>
        [TriggerPropertyContract("Syncronous", "Trigger Syncronous")]
        public bool Syncronous { get; set; }

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
        [TriggerActionContract("{46005D0D-943D-47B5-96D7-4F932EF69034}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                Uri baseAddress = new Uri(URIServiceEndpoint);
                HostController.triggerType = this;
                HostController.actionTrigger = actionTrigger;
                HostController.context = context;

                using (ServiceHost host = new ServiceHost(typeof(HostController), baseAddress))
                {
                    // Enable metadata publishing.
                    ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                    smb.HttpGetEnabled = true;
                    smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                    host.Description.Behaviors.Add(smb);
                    host.Open();
                    Thread.Sleep(Timeout.Infinite);
                }

                return null;
            }
            catch (Exception)
            {
                actionTrigger(this, null);
                return null;
            }
        }
    }
}