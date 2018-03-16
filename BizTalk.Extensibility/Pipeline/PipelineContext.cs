// PipelineContext.cs
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
#region Usings

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Transactions;
using Microsoft.BizTalk.Bam.EventObservation;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Test.BizTalk.PipelineObjects;

#endregion

namespace SnapGate.BizTalk.Extensibility
{
    /// <summary>
    ///     Controls the lifetime and result of a
    ///     pipeline transaction.
    /// </summary>
    /// <remarks>
    ///     Using it is similar to using a TransactionScope object.
    /// </remarks>
    /// <example><![CDATA[
    /// using ( TransactionControl control = pipeline.EnableTransactions() )
    /// {
    ///    // do stuff
    ///    control.SetComplete(); // commit transaction if all ok
    /// }
    /// ]]></example>
    public class TransactionControl : IDisposable
    {
        private bool _complete;
        private CommittableTransaction _transaction;

        internal TransactionControl(CommittableTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            _transaction = transaction;
        }

        /// <summary>
        ///     Remove the transaction context and attempt
        ///     to commit or rollback the transaction.
        /// </summary>
        public void Dispose()
        {
            if (_complete)
                _transaction.Commit();
            else
                _transaction.Rollback();
            _transaction.Dispose();
        }

        /// <summary>
        ///     Marks the transaction to attempt to commit
        ///     during disposal.
        /// </summary>
        public void SetComplete()
        {
            _complete = true;
        }
    } // class TransactionControl

    /// <summary>
    ///     Interface used to configure and update
    ///     the pipeline context mock object
    /// </summary>
    internal interface IConfigurePipelineContext : IPipelineContext, IPipelineContextEx
    {
        void AddDocSpecByName(string name, IDocumentSpec documentSpec);
        void AddDocSpecByType(string type, IDocumentSpec documentSpec);
        void SetAuthenticationRequiredOnReceivePort(bool value);
        void SetGroupSigningCertificate(string certificate);
        TransactionControl EnableTransactionSupport();
    } // IConfigurePipelineContext

    /// <summary>
    ///     Mock class that represents the pipeline
    ///     execution context.
    /// </summary>
    /// <remarks>
    ///     We mock this class explicitly in order to implement
    ///     some functionality that the PipelineObjects library currently
    ///     does not implement, including Transaction and Certificate object
    ///     support
    /// </remarks>
    public class PipelineContext : IPipelineContext, IPipelineContextEx, IConfigurePipelineContext
    {
        private bool _authenticationRequiredOnReceivePort;
        private int _componentIndex = 0;

        private Dictionary<string, IDocumentSpec> _docSpecsByName =
            new Dictionary<string, IDocumentSpec>();

        private Dictionary<string, IDocumentSpec> _docSpecsByType =
            new Dictionary<string, IDocumentSpec>();

        private StreamEventManagement _eventStream;
        private IBaseMessageFactory _messageFactory = new MessageFactory();
        private Guid _pipelineId = Guid.Empty;
        private string _pipelineName = "Pipeline";
        private IResourceTracker _resourceTracker = new ResourceTracker();
        private string _signingCertificate;
        private Guid _stageId = Guid.Empty;
        private int _stageIndex = 0;
        private object _transaction;

        internal class StreamEventManagement : EventStream
        {
            public override void AddReference(string activityName, string activityID, string referenceType,
                string referenceName, string referenceData)
            {
            }

            public override void AddReference(string activityName, string activityID, string referenceType,
                string referenceName, string referenceData, string longreferenceData)
            {
            }

            public override void AddRelatedActivity(string activityName, string activityID, string relatedActivityName,
                string relatedTraceID)
            {
            }

            public override void BeginActivity(string activityName, string activityInstance)
            {
            }

            public override void Clear()
            {
            }

            public override void EnableContinuation(string activityName, string activityInstance,
                string continuationToken)
            {
            }

            public override void EndActivity(string activityName, string activityInstance)
            {
            }

            public override void Flush()
            {
            }

            public override void Flush(SqlConnection connection)
            {
            }

            public override void StoreCustomEvent(IPersistQueryable singleEvent)
            {
            }

            public override void UpdateActivity(string activityName, string activityInstance, params object[] data)
            {
            }
        }

        #region IPipelineContext Members

        //
        // IPipelineContext Members
        //

        /// <summary>
        ///     Index of the currently executing component
        /// </summary>
        public int ComponentIndex
        {
            get { return _componentIndex; }
        }

        /// <summary>
        ///     ID of the currently executing pipeline
        /// </summary>
        public Guid PipelineID
        {
            get { return _pipelineId; }
        }

        /// <summary>
        ///     The pipeline name
        /// </summary>
        public string PipelineName
        {
            get { return _pipelineName; }
        }

        /// <summary>
        ///     The Resource Tracker object associated with this pipeline
        /// </summary>
        public IResourceTracker ResourceTracker
        {
            get { return _resourceTracker; }
        }

        /// <summary>
        ///     The ID of the stage currently executing
        /// </summary>
        public Guid StageID
        {
            get { return _stageId; }
        }

        /// <summary>
        ///     The Index of the stage currently executing
        /// </summary>
        public int StageIndex
        {
            get { return _stageIndex; }
        }

        /// <summary>
        ///     Finds a document specification for a schema added
        ///     to the context
        /// </summary>
        /// <param name="docSpecName">CLR type name of the schema</param>
        /// <returns>The document spec, if it exists</returns>
        public IDocumentSpec GetDocumentSpecByName(string docSpecName)
        {
            if (_docSpecsByName.ContainsKey(docSpecName))
            {
                return _docSpecsByName[docSpecName];
            }
            throw new COMException("Could not locate document specification with name: " + docSpecName);
        }

        /// <summary>
        ///     Finds a document specification for a schema added
        ///     to the context
        /// </summary>
        /// <param name="docType">The XML namespace#root of the schema</param>
        /// <returns>The document spec, if it exists</returns>
        public IDocumentSpec GetDocumentSpecByType(string docType)
        {
            if (_docSpecsByType.ContainsKey(docType))
            {
                return _docSpecsByType[docType];
            }
            throw new COMException("Could not locate document specification with type: " + docType);
        }

        /// <summary>
        ///     Gets the BAM Event Stream for the pipeline.
        /// </summary>
        /// <returns>An empty stream</returns>
        public EventStream GetEventStream()
        {
            if (_eventStream == null)
                _eventStream = new StreamEventManagement();
            return _eventStream;
        }

        /// <summary>
        ///     Gets the thumbprint of the X.509 group
        ///     signing certificate
        /// </summary>
        /// <returns>The certificate thumbprint, or null</returns>
        public string GetGroupSigningCertificate()
        {
            return _signingCertificate;
        }

        /// <summary>
        ///     Gets the message factory object
        /// </summary>
        /// <returns>The mesage factory</returns>
        public IBaseMessageFactory GetMessageFactory()
        {
            return _messageFactory;
        }

        #endregion // IPipelineContext Members

        #region IPipelineContextEx Members

        //
        // IPipelineContextEx Members
        //

        /// <summary>
        ///     If true, indicates authentication on the
        ///     receive port was enabled
        /// </summary>
        public bool AuthenticationRequiredOnReceivePort
        {
            get { return _authenticationRequiredOnReceivePort; }
        }

        /// <summary>
        ///     Gets the transaction object associated with the process
        /// </summary>
        /// <returns>The ITransaction object</returns>
        public object GetTransaction()
        {
            return _transaction;
        }

        #endregion // IPipelineContextEx Members

        #region IConfigurePipelineContext Members

        //
        // IConfigurePipelineContext Members
        //

        /// <summary>
        ///     Adds a new document specification to the context
        /// </summary>
        /// <param name="name">CLR Type name</param>
        /// <param name="documentSpec">Document Spec</param>
        public void AddDocSpecByName(string name, IDocumentSpec documentSpec)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (documentSpec == null)
                throw new ArgumentNullException("documentSpec");

            if (!_docSpecsByName.ContainsKey(name))
                _docSpecsByName.Add(name, documentSpec);
        }

        /// <summary>
        ///     Adds a new document specification to the context
        /// </summary>
        /// <param name="type">XML namespace#root</param>
        /// <param name="documentSpec">Document Spec</param>
        public void AddDocSpecByType(string type, IDocumentSpec documentSpec)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (documentSpec == null)
                throw new ArgumentNullException("documentSpec");

            if (!_docSpecsByType.ContainsKey(type))
                _docSpecsByType.Add(type, documentSpec);
        }

        /// <summary>
        ///     Configures the AuthenticationRequiredOnReceivePort option
        /// </summary>
        /// <param name="value">New value</param>
        public void SetAuthenticationRequiredOnReceivePort(bool value)
        {
            _authenticationRequiredOnReceivePort = value;
        }

        /// <summary>
        ///     Sets the group signing certificate to use
        /// </summary>
        /// <param name="certificate">The certificate thumbprint</param>
        public void SetGroupSigningCertificate(string certificate)
        {
            _signingCertificate = certificate;
        }

        /// <summary>
        ///     Enables a transaction for the pipeline execution
        /// </summary>
        /// <returns>Object to control the transaction lifetime</returns>
        public TransactionControl EnableTransactionSupport()
        {
            CommittableTransaction tx = new CommittableTransaction();
            _transaction = TransactionInterop.GetDtcTransaction(tx);
            return new TransactionControl(tx);
        }

        #endregion // IConfigurePipelineContext Members
    } // class PipelineContext
} // namespace BTSGBizTalkAddins