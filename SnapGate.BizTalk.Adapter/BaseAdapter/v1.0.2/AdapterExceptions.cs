// AdapterExceptions.cs
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
using System.Runtime.Serialization;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace SnapGate.Framework.BizTalk.Adapter.Common
{
	// Adapter Exceptions

	[Serializable()]
	public class AdapterException : ApplicationException
	{
		public AdapterException () { }

		public AdapterException (string message) : base(message) { }

		public AdapterException (Exception inner) : base(String.Empty, inner) { }

		public AdapterException (string msg, Exception e) : base(msg,e) { }

		protected AdapterException (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
    public class AlreadyInitialized : AdapterException
	{
        public AlreadyInitialized() : base("Adapter already initialized.") { }

		protected AlreadyInitialized (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
    public class NotInitialized : AdapterException
	{
		public NotInitialized () : base("Adapter not initialized.") { }

		protected NotInitialized (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
    public class InconsistentConfigurationUri : AdapterException
	{
		public InconsistentConfigurationUri (string uriLocation, string uriConfiguration) : base(
            String.Format("The adapter configuration URI is {0} but the Adapter Framework contains {1} possible corruption in the configuration. Delete and recreate the location.", uriLocation, uriConfiguration)) { }

		protected InconsistentConfigurationUri (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
    public class EndpointExists : AdapterException
	{
		public EndpointExists (string uri) : base(String.Format("The endpoint {0} already exists.", uri)) { }

		protected EndpointExists (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
	
	[Serializable()]
    public class EndpointNotExists : AdapterException
	{
		public EndpointNotExists (string uri) : base(String.Format("The endpoint {0} does not exist.", uri)) { }

		protected EndpointNotExists (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
    public class NoAdapterConfig : AdapterException
	{
		public NoAdapterConfig () : base("No adapter configuration XML was found on the configuration when one was expected.") { }

		protected NoAdapterConfig (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
    public class NoSuchProperty : AdapterException 
	{
		public NoSuchProperty (string path) : base(String.Format("Property {0} not found on adapter configuration XML.", path)) { }

		protected NoSuchProperty (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
    public class CreateEndpointFailed : AdapterException
	{
		public CreateEndpointFailed (string fullName, string uri) : base(String.Format("Unable to create endpoint type {0} location URI {1}", fullName, uri)) { }

		protected CreateEndpointFailed (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
    public class ErrorThresholdExceeded : AdapterException
	{
        public ErrorThresholdExceeded() : base("Error Threshold exceeded") { }

		protected ErrorThresholdExceeded (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
    public class ErrorLoadingConfigXmlDom : AdapterException
	{
        public ErrorLoadingConfigXmlDom() : base("A failure occurred in loading the configuration XML DOM.") { }

		protected ErrorLoadingConfigXmlDom (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
    public class ErrorRetrievingSsoTicket : AdapterException
	{
        public ErrorRetrievingSsoTicket() : base("A failure occurred in retrieving the SSO ticket.") { }

		protected ErrorRetrievingSsoTicket (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable()]
    public class ErrorTransmitUnexpectedClrException : AdapterException
	{
		public ErrorTransmitUnexpectedClrException (string message) : base(String.Format("An unexpected failure occurred while processing a message. The text associated with the exception is \"{0}\".", message)) { }

		protected ErrorTransmitUnexpectedClrException (SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

    [Serializable()]
    public class EventLogErrorThresholdExceeded : AdapterException
    {
        public EventLogErrorThresholdExceeded() : base("The Event Log Error Threshold has been reached. The adapter will continue polling, but further event log entries will be suppressed.") { }

        protected EventLogErrorThresholdExceeded(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable()]
    public class EventLogErrorThresholdReset : AdapterException
    {
        public EventLogErrorThresholdReset() : base("The adapter has recovered from recent failures. The Event Log Error Threshold count will also be reset.") { }
        
        protected EventLogErrorThresholdReset(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
