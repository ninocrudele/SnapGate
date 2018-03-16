// SnapGateTransmitter.cs
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
using Microsoft.BizTalk.TransportProxy.Interop;
using SnapGate.Framework.BizTalk.Adapter.Common;

namespace SnapGate.Framework.BizTalk.Adapter
{
	/// <summary>
	/// This is a singleton class for SnapGate send adapter. All the messages, going to various
	/// send ports of this adapter type, will go through this class.
	/// </summary>
	public class SnapGateTransmitter : AsyncTransmitter
	{
		internal static string SNAPGATE_NAMESPACE = "http://schemas.microsoft.com/BizTalk/2003/SDK_Samples/Messaging/Transports/SnapGate-properties";

		public SnapGateTransmitter() : base(
			"SnapGate Transmit Adapter",
			"1.0",
			"Send messages form BizTalk to SnapGate points",
            "SnapGate-Messaging",
			new Guid("024DB758-AAF9-415e-A121-4AC245DD49EC"),
            SNAPGATE_NAMESPACE,
			typeof(SnapGateTransmitterEndpoint),
            SnapGateTransmitProperties.BatchSize)
        {
		}
	
		protected override void HandlerPropertyBagLoaded ()
		{
			IPropertyBag config = this.HandlerPropertyBag;
			if (null != config)
			{
				XmlDocument handlerConfigDom = ConfigProperties.IfExistsExtractConfigDom(config);
				if (null != handlerConfigDom)
				{
					SnapGateTransmitProperties.ReadTransmitHandlerConfiguration(handlerConfigDom);
				}
			}
		}
	}
}
