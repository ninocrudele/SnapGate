// SnapGateReceiverEndpoint.cs
// 
// BSD 3-Clause License
// 
// Copyright (c) 2014-2016, Nino Crudele <nino dot crudele at live dot com>
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// * Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Text;
using System.Security;
using System.Threading;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.TransportProxy.Interop;
using SnapGate.Framework.BizTalk.Adapter.Common;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Library;


namespace SnapGate.Framework.BizTalk.Adapter
{
    using SnapGate.Framework.Base;
    using SnapGate.Framework.Common;
    using SnapGate.Framework.Contracts.Events;
    using SnapGate.Framework.Contracts.Globals;
    using SnapGate.Framework.Library;

    /// <summary>
    /// This class corresponds to a Receive Location/URI.  It handles polling the
    /// given folder for new messages.
    /// </summary>
    internal class SnapGateReceiverEndpoint : ReceiverEndpoint
    {
        // constants
        private const string MESSAGE_BODY = "body";
        private const string PROP_REMOTEMESSAGEID = "RemoteMessageId";
        private const string PROP_IDCONFIGURATION = "idConfiguration";
        private const string PROP_IDCOMPONENT = "idComponent";
        private const string PROP_POINTNAME = "pointName";
        private const string PROP_NAMESPACE = "https://SnapGate.BizTalk.Schemas.SnapGateProperties";

        //  properties
        private SnapGateReceiveProperties properties;

        //  handle to the EPM
        private IBTTransportProxy transportProxy;

        // used to create new messages / message parts etc.
        private IBaseMessageFactory messageFactory;

        //  used in the creation of messages
        private string transportType;

        //  used in the creation of messages
        private string propertyNamespace;

        // used to track inflight work
        private ControlledTermination controlledTermination;

        //  error count for comparison with the error threshold
        int errorCount;
        //  support for Update
        IPropertyBag updatedConfig;
        IPropertyBag updatedBizTalkConfig;
        IPropertyBag updatedHandlerPropertyBag;

        public SnapGateReceiverEndpoint()
        {
        }

        /// <summary>
        /// This method is called when a Receive Location is enabled.
        /// </summary>
        public override void Open(
            string uri,
            IPropertyBag config,
            IPropertyBag bizTalkConfig,
            IPropertyBag handlerPropertyBag,
            IBTTransportProxy transportProxy,
            string transportType,
            string propertyNamespace,
            ControlledTermination control)
        {
            Trace.WriteLine("[SnapGateReceiverEndpoint] Open called");
            this.errorCount = 0;

            this.properties = new SnapGateReceiveProperties();
            //  Location properties - possibly override some Handler properties
            XmlDocument locationConfigDom = ConfigProperties.ExtractConfigDom(config);
            this.properties.ReadLocationConfiguration(locationConfigDom);

            //  this is our handle back to the EPM
            this.transportProxy = transportProxy;

            // used to create new messages / message parts etc.
            this.messageFactory = this.transportProxy.GetMessageFactory();

            //  used in the creation of messages
            this.transportType = transportType;

            //  used in the creation of messages
            this.propertyNamespace = propertyNamespace;

            // used to track inflight work for shutting down properly
            this.controlledTermination = control;

            //start the task
            StartSnapGate();
        }

        /// <summary>
        /// This method is called when the configuration for this receive location is modified.
        /// </summary>
        public override void Update (IPropertyBag config, IPropertyBag bizTalkConfig, IPropertyBag handlerPropertyBag)
        {
            Trace.WriteLine("[SnapGateReceiverEndpoint] Updated called");
            lock (this)
            {

                errorCount = 0;

                //  keep handles to these property bags until we are ready
                this.updatedConfig             = config;
                this.updatedBizTalkConfig      = bizTalkConfig;
                this.updatedHandlerPropertyBag = handlerPropertyBag;

                if (updatedConfig != null)
                {
                    XmlDocument locationConfigDom = ConfigProperties.ExtractConfigDom(this.updatedConfig);
                    this.properties.ReadLocationConfiguration(locationConfigDom);
                }

                //Schedule the polling event
                //need to run something to upgate SnapGate?
                //I'm going to remove the update for now
            }
        }

        /// <summary>
        /// Stop engine
        /// </summary>
        public override void Dispose()
        {
            try
            {
                Trace.WriteLine("BizTalk adapter stopping - SnapGate Embedded Library disposing");
                MainThread.Abort();
                Trace.WriteLine("BizTalk adapter stopped - SnapGate Embedded Library stopped");
            }
            catch (Exception e)
            {
                Trace.WriteLine($"BizTalk adapter stopping error: {e.Message}");
                throw;
            }


        }
        /// <summary>
        /// The set event action event embedded.
        /// </summary>
        private static Embedded.SetEventActionEventEmbedded setEventActionEventEmbedded;

        /// <summary>
        /// Main process thread
        /// </summary>
        private Thread MainThread { get; set; }

        /// <summary>
        /// start engine and new thread
        /// </summary>
        private void StartSnapGate()
        {
            try
            {
                setEventActionEventEmbedded = EventReceivedFromEmbedded;
                Embedded.setEventActionEventEmbedded = setEventActionEventEmbedded;
                Trace.WriteLine("Start SnapGate Embedded Library for BizTalk Adapter");

                MainThread = new Thread(Embedded.StartEngine);
                MainThread.Start();
                Trace.WriteLine("SnapGate Embedded Library for BizTalk Adapter started");

                //Success, reset the error count
                errorCount = 0;
            }
            catch (Exception e)
            {
                transportProxy.SetErrorInfo(e);
                //Track number of failures
                errorCount++;
            }
        }


        /// <summary>
        /// The event received from embedded.
        /// </summary>
        /// <param name="eventType">
        /// The event type.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        private void EventReceivedFromEmbedded(IEventType eventType, ActionContext context)
        {
            string stringValue = Encoding.UTF8.GetString(eventType.DataContext);
            Trace.WriteLine("---------------EVENT RECEIVED FROM EMBEDDED LIBRARY---------------");

            PrepareMessageAndSubmit(eventType, context);
            Trace.WriteLine("---------------EVENT RECEIVED FROM EMBEDDED LIBRARY---------------");
            Trace.WriteLine(stringValue);

        }

        List<BatchMessage> batchMessages = new List<BatchMessage>();
        //  The algorithm implemented here splits the list of messages according to the
        //  batch tuning parameters (number of bytes and number of messages) because the
        //  list is randomly ordered it is possible to have non-optimal batches. It would
        //  be a slight optimization to order by increasing size and then cut the batches.
        private void PrepareMessageAndSubmit (IEventType eventType, ActionContext context)
        {
            Trace.WriteLine("[SnapGateReceiverEndpoint] PrepareMessageAndSubmit called");
            
            int maxBatchSize     = this.properties.MaximumBatchSize;
            int maximumNumberOfMessages = this.properties.MaximumNumberOfMessages;

            
            long bytesInBatch = 0;
            
            IBaseMessage msg = CreateMessage(eventType, context);
            if ( null == msg )
                return;
            else
                batchMessages.Add(new BatchMessage(msg, context.BubblingObjectBag.MessageId, BatchOperationType.Submit));            //  keep a running total for the current batch
            bytesInBatch += eventType.DataContext.Length;

            //  zero for the value means infinite 
            bool messagesCountExceeded = ((0 != maximumNumberOfMessages) && (batchMessages.Count >= maximumNumberOfMessages));
            bool byteCountExceeded = ((0 != maxBatchSize)     && (bytesInBatch    >  maxBatchSize));

            if (messagesCountExceeded || byteCountExceeded)
            {
                //  check if we have been asked to stop - if so don't start another batch
                if (this.controlledTermination.TerminateCalled)
                    return;

                //  execute the batch
                this.SubmitMessages(batchMessages);

                //  reset the running totals
                bytesInBatch = 0;
                batchMessages.Clear();
            }
            

            //  check if we have been asked to stop - if so don't start another batch
            if (this.controlledTermination.TerminateCalled)
                return;

            //  end of message list - one final batch to do
            if (batchMessages.Count > 0)
                this.SubmitMessages(batchMessages);
        }

        /// <summary>
        /// Given a List of Messages submit them to BizTalk for processing
        /// </summary>
        private void SubmitMessages(List<BatchMessage> items)
        {
            if (items == null || items.Count == 0) throw new ArgumentException("SubmitMessages was called with an empty list of messages");

            Trace.WriteLine(string.Format("[SnapGateReceiverEndpoint] SubmitMessages called. Submitting a batch of {0} messages.", items.Count));

            //This class is used to track the messages associated with this ReceiveBatch
            BatchInfo batchInfo = new BatchInfo(items);
            using (ReceiveBatch batch = new ReceiveBatch(this.transportProxy, this.controlledTermination, batchInfo.OnBatchComplete, this.properties.MaximumNumberOfMessages))
            {
                foreach (BatchMessage item in items)
                {
                    // submit message to batch
                    batch.SubmitMessage(item.Message, item.UserData);
                }

                batch.Done(null);
            }
        }

        /// <summary>
        /// This class tracks a collection of messages associated with a EPM Batch
        /// </summary>
        private class BatchInfo
        {
            BatchMessage[] messages;

            internal BatchInfo(List<BatchMessage> messageList)
            {
                this.messages = new BatchMessage[messageList.Count];
                messageList.CopyTo(messages);
            }

            /// <summary>
            /// Called when the BizTalk Batch has been submitted.  If all the messages were submitted (good or suspended)
            /// we delete the messages from the folder
            /// </summary>
            /// <param name="overallStatus"></param>
            internal void OnBatchComplete(bool overallStatus)
            {
                Trace.WriteLine(string.Format("[SnapGateReceiverEndpoint] OnBatchComplete called. overallStatus == {0}.", overallStatus));

                if (overallStatus == true) //Batch completed
                {
                    //Delete the messages
                    foreach (BatchMessage batchMessage in messages)
                    {
                        //Close the stream so we can delete this message
                        batchMessage.Message.BodyPart.Data.Close();
                    }
                }
            }
        }

        private IBaseMessage CreateMessage (IEventType eventType, ActionContext contextItem)
        {
            Stream fs;
            fs = new MemoryStream(eventType.DataContext);
            IBaseMessagePart part = this.messageFactory.CreateMessagePart();
            part.Data = fs;
            IBaseMessage message = this.messageFactory.CreateMessage();
            message.AddPart(MESSAGE_BODY, part, true);

            SystemMessageContext context = new SystemMessageContext(message.Context);
            context.InboundTransportType     = this.transportType;
            context.InboundTransportLocation = this.properties.Uri;
            //Write/Promote any adapter specific properties on the message context
            message.Context.Write(PROP_REMOTEMESSAGEID, PROP_NAMESPACE, contextItem.BubblingObjectBag.MessageId);
            
            return message;
        }


    }
}
