// SnapGateReceiver.cs
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
using Microsoft.BizTalk.Component.Interop;
using SnapGate.Framework.BizTalk.Adapter.Common;

namespace SnapGate.Framework.BizTalk.Adapter
{
    public class SnapGateReceiver : Receiver 
    {
        public SnapGateReceiver() : base(
            "SnapGate Receive Adapter",
            "1.0",
            "Submits message from SnapGate points into BizTalk",
            "SnapGate-Messaging",
            new Guid("3D4B599E-2202-4bbb-9FC6-7ACA3906E5DE"),
            "http://schemas.microsoft.com/BizTalk/2003/SnapGate-properties",
            typeof(SnapGateReceiverEndpoint))
        {
        }
        /// <summary>
        /// This function is called when BizTalk runtime gives the handler properties to adapter.
        /// </summary>
        protected override void HandlerPropertyBagLoaded ()
        {
            IPropertyBag config = this.HandlerPropertyBag;
            if (null != config)
            {
                XmlDocument handlerConfigDom = ConfigProperties.IfExistsExtractConfigDom(config);
                if (null != handlerConfigDom)
                {
                    SnapGateReceiveProperties.ReceiveHandlerConfiguration(handlerConfigDom);
                }
            }
        }
    }
}
