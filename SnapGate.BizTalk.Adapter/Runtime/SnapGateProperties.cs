// SnapGateProperties.cs
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
using System.Xml;
using System.Net;
using Microsoft.BizTalk.Message.Interop;
using SnapGate.Framework.BizTalk.Adapter.Common;

namespace SnapGate.Framework.BizTalk.Adapter
{
    using System.Data;
    using System.Diagnostics;

    using SnapGate.Framework.Base;
    using SnapGate.Framework.Common;

    /// <summary>
    /// This class handles properties for a given Receive Location
    /// </summary>
    internal class SnapGateReceiveProperties : ConfigProperties
    {
        // Handler properties
        private static int handlerMaximumNumberOfMessages = 0;

        // Endpoint properties
        private string pointName;
        private int maximumBatchSize;
        private int maximumNumberOfMessages;
        private int errorThreshold;
        private string workInProgress;
        private string uri;

        public string PointName { get { return this.pointName; } }
        public int MaximumBatchSize { get { return maximumBatchSize; } }
        public int MaximumNumberOfMessages { get { return this.maximumNumberOfMessages; } }
        public int ErrorThreshold { get { return errorThreshold; } }
        public string Uri { get { return uri; } }

        public SnapGateReceiveProperties() : base()
        {
            // establish defaults
            this.pointName = String.Empty;
            maximumBatchSize = 0;
            this.maximumNumberOfMessages = handlerMaximumNumberOfMessages; // default to handler value, override if set on the endpoint
            workInProgress = String.Empty;
        }

        /// <summary>
        /// Load the Configuration for the Receive Handler
        /// </summary>
        public static void ReceiveHandlerConfiguration(XmlDocument configDOM)
        {
            // Handler properties
            handlerMaximumNumberOfMessages = IfExistsExtractInt(configDOM, "/Config/maximumNumberOfMessages", handlerMaximumNumberOfMessages);
        }

        /// <summary>
        /// Load the Configuration for a Receive Location
        /// </summary>
        public void ReadLocationConfiguration(XmlDocument configDOM)
        {

            this.pointName = IfExistsExtract(configDOM, "/Config/pointName", string.Empty);
            this.maximumBatchSize = IfExistsExtractInt(configDOM, "/Config/maximumBatchSize", 0);
            this.maximumNumberOfMessages = IfExistsExtractInt(configDOM, "/Config/maximumNumberOfMessages", handlerMaximumNumberOfMessages);
            this.errorThreshold = ExtractInt(configDOM, "/Config/errorThreshold");
            this.workInProgress = IfExistsExtract(configDOM, "/Config/workInProgress", string.Empty);
            this.uri = IfExistsExtract(configDOM, "/Config/uri", string.Empty);
        }

    }

    /// <summary>
	/// This class maintains send port properties associated with a message. These properties
	/// will be extracted from the message context for static send ports.
	/// </summary>
    internal class SnapGateTransmitProperties : ConfigProperties
    {
        // Handler properties
        private static int handlerSendBatchSize = 20;
        private static int handlerbufferSize = 4096;
        private static int handlerthreadsPerCPU = 1;

        // Endpoint properties
        private string idConfiguration;
        private string idComponent;
        private string uri;

        public string IdConfiguration { get { return idConfiguration; } }
        public string IdComponent { get { return idComponent; } }
        public string Uri { get { return uri; } }
        public static int BatchSize { get { return handlerSendBatchSize; } }

        public SnapGateTransmitProperties(IBaseMessage message, string propertyNamespace)
        {

            XmlDocument locationConfigDom = null;

            //  get the adapter configuration off the message
            IBaseMessageContext context = message.Context;
            string config = (string)context.Read("AdapterConfig", propertyNamespace);

            //  the config can be null all that means is that we are doing a dynamic send
            if (null != config)
            {
                locationConfigDom = new XmlDocument();
                locationConfigDom.LoadXml(config);

                this.ReadLocationConfiguration(locationConfigDom);
            }
        }

        /// <summary>
        /// Load the Transmit Handler configuration settings
        /// </summary>
        public static void ReadTransmitHandlerConfiguration(XmlDocument configDOM)
        {

            // Handler properties
            handlerSendBatchSize = ExtractInt(configDOM, "/Config/sendBatchSize");
            handlerbufferSize = ExtractInt(configDOM, "/Config/bufferSize");
            handlerthreadsPerCPU = ExtractInt(configDOM, "/Config/threadsPerCPU");
        }

        /// <summary>
        /// Load the configuration for the Message that is being transmitted
        /// </summary>
        /// <param name="configDOM"></param>
        public void ReadLocationConfiguration(XmlDocument configDOM)
        {


            this.idConfiguration = Extract(configDOM, "/Config/idConfiguration", string.Empty);
            this.idComponent = Extract(configDOM, "/Config/idComponent", string.Empty);
            uri = Extract(configDOM, "/Config/uri", string.Empty);

        }
    }
}

