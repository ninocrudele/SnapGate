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
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;

#endregion

namespace SnapGate.Framework.AzureQueueEvent
{
    /// <summary>
    ///     The azure queue event.
    /// </summary>
    [EventContract("{628CB14D-7F85-4D99-8EC6-489EBA25C38A}", "AzureQueueEvent", "Send message to Azure Queue", true)]
    public class AzureQueueEvent : IEventType
    {
        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        [EventPropertyContract("ConnectionString", "Azure ConnectionString")]
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets the queue path.
        /// </summary>
        [EventPropertyContract("QueuePath", "QueuePath")]
        public string QueuePath { get; set; }

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
        [EventActionContract("{287F60BE-B257-4EE8-B3C3-328D0AFCD692}", "Main action", "Main action description")]
        public async Task Execute(ActionEvent actionEvent, ActionContext context)
        {
            try
            {
                Context = context;
                ActionEvent = actionEvent;

                var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);

                if (!namespaceManager.QueueExists(QueuePath))
                {
                    namespaceManager.CreateQueue(QueuePath);
                }

                var client = QueueClient.CreateFromConnectionString(ConnectionString, QueuePath);
                client.Send(new BrokeredMessage(DataContext));
                actionEvent(this, context);
                return;
            }
            catch
            {
                // ignored
                return;
            }
        }
    }
}