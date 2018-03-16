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
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace SnapGate.Framework.AzureTopicTrigger
{
    /// <summary>
    ///     The azure topic trigger.
    /// </summary>
    [TriggerContract("{D56A660E-2BBE-4705-BA2E-E89BBE0689DB}", "Azure Topic Trigger", "Azure Topic Trigger", false,
        true,
        false)]
    public class AzureTopicTrigger : ITriggerType
    {
        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        [TriggerPropertyContract("ConnectionString", "Azure ConnectionString")]
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets the topic path.
        /// </summary>
        [TriggerPropertyContract("TopicPath", "TopicPath")]
        public string TopicPath { get; set; }

        /// <summary>
        ///     Gets or sets the messages filter.
        /// </summary>
        [TriggerPropertyContract("MessagesFilter", "MessagesFilter")]
        public string MessagesFilter { get; set; }

        /// <summary>
        ///     Gets or sets the subscription name.
        /// </summary>
        [TriggerPropertyContract("SubscriptionName", "SubscriptionName")]
        public string SubscriptionName { get; set; }

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
        [TriggerActionContract("{EB36D04B-7491-46EF-B27F-6F07E2F31D48}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);

                if (!namespaceManager.TopicExists(TopicPath))
                {
                    namespaceManager.CreateTopic(TopicPath);
                }

                var sqlFilter = new SqlFilter(MessagesFilter);

                if (!namespaceManager.SubscriptionExists(TopicPath, SubscriptionName))
                {
                    namespaceManager.CreateSubscription(TopicPath, SubscriptionName, sqlFilter);
                }

                var subscriptionClientHigh = SubscriptionClient.CreateFromConnectionString(
                    ConnectionString,
                    TopicPath,
                    SubscriptionName);

                // Configure the callback options
                var options = new OnMessageOptions {AutoComplete = false, AutoRenewTimeout = TimeSpan.FromMinutes(1)};

                // Callback to handle received messages
                subscriptionClientHigh.OnMessage(
                    message =>
                    {
                        try
                        {
                            // Remove message from queue
                            message.Complete();
                            DataContext = message.GetBody<byte[]>();
                            actionTrigger(this, context);
                        }
                        catch (Exception)
                        {
                            // Indicates a problem, unlock message in queue
                            message.Abandon();
                        }
                    },
                    options);
                return null;
            }
            catch (Exception)
            {
                // ignored
                return null;
            }
        }
    }
}