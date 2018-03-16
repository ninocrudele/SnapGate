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
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;

#endregion

namespace SnapGate.Framework.AzureTopicEvent
{
    /// <summary>
    ///     The azure topic event.
    /// </summary>
    [EventContract("{B2311010-B505-4F9F-A927-2035A7640BCB}", "AzureTopicEvent", "Send message to Azure Topic", true)]
    public class AzureTopicEvent : IEventType
    {
        /// <summary>
        ///     Gets or sets the connection string.
        /// </summary>
        [EventPropertyContract("ConnectionString", "Azure ConnectionString")]
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Gets or sets the topic path.
        /// </summary>
        [EventPropertyContract("TopicPath", "TopicPath")]
        public string TopicPath { get; set; }

        /// <summary>
        ///     Gets or sets the message context properties.
        /// </summary>
        [EventPropertyContract("MessageContextProperties", "MessageContextProperties")]
        public string MessageContextProperties { get; set; }

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
        [EventActionContract("{D33251EF-7638-4C34-AD6B-B5CBE32F7056}", "Main action", "Main action description")]
        public async Task Execute(ActionEvent actionEvent, ActionContext context)
        {
            try
            {
                Context = context;
                ActionEvent = actionEvent;

                var namespaceManager = NamespaceManager.CreateFromConnectionString(ConnectionString);

                if (!namespaceManager.TopicExists(TopicPath))
                {
                    namespaceManager.CreateTopic(TopicPath);
                }

                var client = TopicClient.CreateFromConnectionString(ConnectionString, TopicPath);
                var brokeredMessage = new BrokeredMessage(DataContext);

                var value = MessageContextProperties.Split('|');
                brokeredMessage.Properties[value[0]] = value[1];
                client.Send(brokeredMessage);
                actionEvent(this, context);
                return;
            }
            catch (Exception)
            {
                // ignored
                return;
            }
        }
    }
}