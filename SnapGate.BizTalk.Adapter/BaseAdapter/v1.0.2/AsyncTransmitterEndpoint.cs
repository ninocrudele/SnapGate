// AsyncTransmitterEndpoint.cs
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
using System.Xml;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Component.Interop;

namespace SnapGate.Framework.BizTalk.Adapter.Common
{
    public abstract class EndpointParameters
    {
        public abstract string SessionKey { get; }
        public string OutboundLocation { get { return this.outboundLocation; } }
        public EndpointParameters (string outboundLocation)
        {
            this.outboundLocation = outboundLocation;
        }
        protected string outboundLocation;
    }

    internal class DefaultEndpointParameters : EndpointParameters
    {
        public override string SessionKey 
        {
            //  the SessionKey is the outboundLocation in the default case
            get { return this.outboundLocation; }
        }
        public DefaultEndpointParameters (string outboundLocation) : base(outboundLocation)
        {
        }
    }

    public abstract class AsyncTransmitterEndpoint : System.IDisposable
    {
        public AsyncTransmitterEndpoint(AsyncTransmitter transmitter) { }

        public virtual bool ReuseEndpoint { get { return true; } }
        public abstract void Open (EndpointParameters endpointParameters, IPropertyBag handlerPropertyBag, string propertyNamespace);
        public abstract IBaseMessage ProcessMessage (IBaseMessage message);
        public virtual void Dispose () { }
    }
}
