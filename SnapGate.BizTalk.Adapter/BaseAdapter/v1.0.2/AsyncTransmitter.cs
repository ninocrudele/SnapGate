// AsyncTransmitter.cs
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
using System.Runtime.InteropServices;
using System.Collections;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.TransportProxy.Interop;

namespace SnapGate.Framework.BizTalk.Adapter.Common
{
    /// <summary>
    /// This is a singleton class for the send adapter. This send adapter support batched
    /// transmission. Messages will be delivered to this adapter for transmission by the 
    /// messaging engine using adapter batch. Adapter batch (IBTBatchTransmitter) should not
    /// be confused with messaging engine's batch (IBTTransportBatch).
    /// 
    /// Send adapter should have an endpoint class deriving AsyncTransmitterEndpoint class.
    /// When a message is given to the adapter, adapter needs to decide which endpoint the
    /// message should go to. The default implementation of the adapter routes the message
    /// based on "OutboundTransmitLocation" property. However, it can be customized to include 
    /// other properties by overriding CreateEndPointParameters() in this class.
    /// 
    /// See "AsyncTransmitterBatch" class for more details on batching.
    /// </summary>
    public class AsyncTransmitter :
        Adapter,
        System.IDisposable,
        IBTBatchTransmitter
    {
        //  default magic number
        private const int MAX_BATCH_SIZE = 50;

        //  members to initialize the batch with
        private int maxBatchSize = MAX_BATCH_SIZE;
        private Type endpointType;
        private Hashtable endpoints = new Hashtable();

        private ControlledTermination control;

        protected AsyncTransmitter (
            string name,
            string version,
            string description,
            string transportType,
            Guid clsid,
            string propertyNamespace,
            Type endpointType,
            int maxBatchSize)
            : base(
            name,
            version,
            description,
            transportType,
            clsid,
            propertyNamespace)
        {
            this.endpointType = endpointType;
            this.maxBatchSize = maxBatchSize;
            this.control = new ControlledTermination();
        }

        protected virtual int MaxBatchSize
        {
            get { return this.maxBatchSize; }
        }

        protected Type EndpointType
        {
            get { return this.endpointType; }
        }

        protected ControlledTermination ControlledTermination { get { return this.control; } }

        public void Dispose()
        {
            control.Dispose();
        }

        // IBTBatchTransmitter
        public IBTTransmitterBatch GetBatch ()
        { 
            IBTTransmitterBatch tb = CreateAsyncTransmitterBatch();

            return tb;
        }

        protected virtual IBTTransmitterBatch CreateAsyncTransmitterBatch ()
        {
            return new AsyncTransmitterBatch(
                this.MaxBatchSize,
                this.EndpointType,
                this.PropertyNamespace,
                this.HandlerPropertyBag,
                this.TransportProxy,
                this);
        }

        // Endpoint management is the responsibility of the transmitter
        protected virtual EndpointParameters CreateEndpointParameters(IBaseMessage message)
        {
            SystemMessageContext context = new SystemMessageContext(message.Context);
            return new DefaultEndpointParameters(context.OutboundTransportLocation);
        }

        public virtual AsyncTransmitterEndpoint GetEndpoint(IBaseMessage message)
        {
            // Provide a virtual "CreateEndpointParameters" method to map message to endpoint
            EndpointParameters endpointParameters = CreateEndpointParameters(message);

            lock (endpoints)
            {
                AsyncTransmitterEndpoint endpoint = (AsyncTransmitterEndpoint)endpoints[endpointParameters.SessionKey];
                if (null == endpoint)
                {
                    //  we haven't seen this location so far this batch so make a new endpoint
                    endpoint = (AsyncTransmitterEndpoint)Activator.CreateInstance(this.endpointType, new object[] { this });

                    if (null == endpoint)
                        throw new CreateEndpointFailed(this.endpointType.FullName, endpointParameters.OutboundLocation);

                    endpoint.Open(endpointParameters, this.HandlerPropertyBag, this.PropertyNamespace);

                    if (endpoint.ReuseEndpoint)
                    {
                        endpoints[endpointParameters.SessionKey] = endpoint;
                    }
                }
                return endpoint;
            }
        }

        public override void Terminate ()
        {
            try
            {
                //  Block until we are done...
                // Let all endpoints finish the work they are doing before disposing them
                this.control.Terminate();

                foreach (AsyncTransmitterEndpoint endpoint in endpoints.Values)
                {
                    //  clean up and potentially close any endpoints
                    try
                    {
                        endpoint.Dispose();
                    }
                    catch (Exception e)
                    {
                        this.TransportProxy.SetErrorInfo(e);
                    }
                }

                base.Terminate();
            }
            finally
            {
                this.Dispose();
            }
        }

        public bool Enter ()
        {
            return this.control.Enter();
        }

        public void Leave ()
        {
            this.control.Leave();
        }
    }
}
