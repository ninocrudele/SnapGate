// Adapter.cs
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
using System.Diagnostics;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.TransportProxy.Interop;

namespace SnapGate.Framework.BizTalk.Adapter.Common
{
	/// <summary>
	/// Summary description for Adapter.
	/// </summary>
	public abstract class Adapter :
		IBTTransport,
		IBTTransportControl,
		IPersistPropertyBag
	{
		//  core member data
		private string propertyNamespace;
		private IBTTransportProxy transportProxy;
		private IPropertyBag handlerPropertyBag;
		private bool initialized;

		//  member data for implementing IBTTransport
		private string name;
		private string version;
		private string description;
		private string transportType;
		private Guid   clsid;

		protected Adapter (
			string name,
			string version,
			string description,
			string transportType,
			Guid clsid,
			string propertyNamespace)
		{
			Trace.WriteLine(String.Format("Adapter.Adapter name: {0}", name));

			this.transportProxy     = null;
			this.handlerPropertyBag = null;
			this.initialized        = false;

			this.name               = name;
			this.version            = version;
			this.description        = description;
			this.transportType      = transportType;
			this.clsid              = clsid;

			this.propertyNamespace  = propertyNamespace;
		}

		protected string            PropertyNamespace  { get { return propertyNamespace; } }
		public IBTTransportProxy    TransportProxy     { get { return transportProxy; } }
		protected IPropertyBag      HandlerPropertyBag { get { return handlerPropertyBag; } }
		protected bool              Initialized        { get { return initialized; } }

		//  IBTTransport
		public string Name { get { return name; } }
		public string Version { get { return version; } }
		public string Description { get { return description; } }
		public string TransportType { get { return transportType; } }
		public Guid ClassID { get { return clsid; } }

		//  IBTransportControl
		public virtual void Initialize (IBTTransportProxy transportProxy)
		{
            Trace.WriteLine("Adapter.Initialize");

			//  this is a Singleton and this should only ever be called once
			if (this.initialized)
				throw new AlreadyInitialized();				

			this.transportProxy = transportProxy;
			this.initialized = true;
		}
		public virtual void Terminate ()
		{
            Trace.WriteLine("Adapter.Terminate");

			if (!this.initialized)
				throw new NotInitialized();
			
			this.transportProxy = null;
		}

		protected virtual void HandlerPropertyBagLoaded ()
		{
			// let any derived classes know the property bag has now been loaded
		}

		// IPersistPropertyBag
		public void GetClassID (out Guid classid) { classid = this.clsid; }
		public void InitNew () { }
		public void Load (IPropertyBag pb, int pErrorLog)
		{
            Trace.WriteLine("Adapter.Load");
            
            this.handlerPropertyBag = pb;
			HandlerPropertyBagLoaded();
		}
		public void Save (IPropertyBag pb, bool fClearDirty, bool fSaveAllProperties) { }
	}
}
