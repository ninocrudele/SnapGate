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

namespace SnapGate.Framework.AzureQueueTrigger
{
    /// <summary>
    ///     The azure queue trigger.
    /// </summary>
    [TriggerContract("{79F1CAB1-6E78-4BF9-8D2E-F15E87F605CA}", "Azure Queue Trigger", "Azure Queue Trigger", false,
        true,
        false)]
    public class AzureQueueTrigger : ITriggerType
    {
        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        [TriggerPropertyContract("ConnectionString", "Azure ConnectionString")]
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets the queue path.
        /// </summary>
        [TriggerPropertyContract("QueuePath", "QueuePath")]
        public string QueuePath { get; set; }

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
        [TriggerActionContract("{647FE4E4-2FD0-4AF4-8FC2-B3019F0BA571}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);

                if (!namespaceManager.QueueExists(QueuePath))
                {
                    namespaceManager.CreateQueue(QueuePath);
                }

                var client = QueueClient.CreateFromConnectionString(ConnectionString, QueuePath);

                // Configure the callback options
                var options = new OnMessageOptions {AutoComplete = false, AutoRenewTimeout = TimeSpan.FromMinutes(1)};

                // Callback to handle received messages
                client.OnMessage(
                    message =>
                    {
                        try
                        {
                            // Remove message from queue
                            DataContext = message.GetBody<byte[]>();
                            message.Complete();
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