// BasePipelineWrapper.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using SnapGate.Framework.Base;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.Test.BizTalk.PipelineObjects;
using Microsoft.XLANGs.BaseTypes;
using SnapGate.Framework.Log;
using DocumentSpec = Microsoft.BizTalk.Component.Interop.DocumentSpec;
using PStage = Microsoft.Test.BizTalk.PipelineObjects.Stage;

#endregion

namespace SnapGate.BizTalk.Extensibility
{
    /// <summary>
    ///     Wrapper around a pipeline you can execute
    /// </summary>
    public abstract class BWPipeline : IEnumerable<IBaseComponent>
    {
        private bool _isReceivePipeline;
        private IPipeline _pipeline;
        private IPipelineContext _pipelineContext;

        /// <summary>
        ///     Initializes an instance
        /// </summary>
        /// <param name="pipeline">Pipeline object to wrap</param>
        /// <param name="isReceivePipeline">True if it's a receive pipeline</param>
        protected BWPipeline(IPipeline pipeline, bool isReceivePipeline)
        {
            if (pipeline == null)
                throw new ArgumentNullException("pipeline");
            _pipeline = pipeline;
            _pipelineContext = CreatePipelineContext();
            _isReceivePipeline = isReceivePipeline;
        }

        #region IEnumerable<IBaseComponent> Members

        IEnumerator<IBaseComponent> IEnumerable<IBaseComponent>.GetEnumerator()
        {
            foreach (PStage stage in _pipeline.Stages)
            {
                IEnumerator enumerator = stage.GetComponentEnumerator();
                while (enumerator.MoveNext())
                {
                    yield return (IBaseComponent) enumerator.Current;
                }
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IBaseComponent>) this).GetEnumerator();
        }

        #endregion

        /// <summary>
        ///     Adds a component to the specified stage
        /// </summary>
        /// <param name="component">Component to add to the stage</param>
        /// <param name="stage">Stage to add it to</param>
        public void AddComponent(IBaseComponent component, PpStage stage)
        {
            if (component == null)
                throw new ArgumentNullException("component");
            if (stage == null)
                throw new ArgumentNullException("stage");

            if (stage.IsReceiveStage != _isReceivePipeline)
                throw new ArgumentException("Invalid Stage", "stage");

            PStage theStage = FindStage(stage);
            theStage.AddComponent(component);
        }

        /// <summary>
        ///     Adds a new document specification to the list
        ///     of Known Schemas for this pipeline.
        /// </summary>
        /// <remarks>
        ///     Adding known schemas is necessary so that
        ///     document type resolution works in the disassembler/assembler
        ///     stages
        /// </remarks>
        /// <param name="schemaType">Type of the document schema to add</param>
        public void AddDocSpec(Type schemaType)
        {
            if (schemaType == null)
            {
                //16042014
                return;
                //16042014
                throw new ArgumentNullException("schemaType");
            }

            Type[] roots = GetSchemaRoots(schemaType);
            foreach (Type root in roots)
            {
                IDocumentSpec docSpec = LoadDocSpec(root);
                AddDocSpecToContext(docSpec);
            }
        }

        /// <summary>
        ///     Adds a new document specification to the list
        ///     of Known Schemas for this pipeline.
        /// </summary>
        /// <remarks>
        ///     Adding known schemas is necessary so that
        ///     document type resolution works in the disassembler/assembler
        ///     stages. Notice that this overload does NOT do automatic checking
        ///     for multiple roots.
        /// </remarks>
        /// <param name="typeName">
        ///     The fully qualified (namespace.class) name of
        ///     the schema
        /// </param>
        /// <param name="assemblyName">
        ///     The partial or full name of the assembly
        ///     containing the schema
        /// </param>
        public void AddDocSpec(string typeName, string assemblyName)
        {
            if (String.IsNullOrEmpty(typeName))
                throw new ArgumentNullException("typeName");
            if (String.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException("assemblyName");

            IDocumentSpec spec = LoadDocSpec(typeName, assemblyName);
            AddDocSpecToContext(spec);
        }

        /// <summary>
        ///     Returns the document spec object for a known doc
        ///     spec given the fully qualified type name
        /// </summary>
        /// <param name="name">Typename of the schema</param>
        /// <returns>The docSpec object</returns>
        public IDocumentSpec GetKnownDocSpecByName(string name)
        {
            return Context.GetDocumentSpecByName(name);
        }

        /// <summary>
        ///     Returns the document spec object for a known doc
        ///     spec given the name of the root (namespace#root)
        /// </summary>
        /// <param name="name">Name of the root</param>
        /// <returns>The docSpec object</returns>
        public IDocumentSpec GetKnownDocSpecByType(string name)
        {
            return Context.GetDocumentSpecByType(name);
        }

        /// <summary>
        ///     Enables transactional support for the pipeline
        ///     execution, so that the pipeline context
        ///     returns a valid transaction
        /// </summary>
        /// <returns>An object to control the transaction lifetime and result</returns>
        public TransactionControl EnableTransactions()
        {
            IConfigurePipelineContext ctxt = (IConfigurePipelineContext) _pipelineContext;
            return ctxt.EnableTransactionSupport();
        }

        /// <summary>
        ///     Looks up a component in the pipeline
        /// </summary>
        /// <param name="stage">The stage the component is in</param>
        /// <param name="index">The 0-based index inside the stage</param>
        /// <returns>The component, or null if it was not found</returns>
        public IBaseComponent GetComponent(PpStage stage, int index)
        {
            foreach (PStage st in _pipeline.Stages)
            {
                if (st.Id == stage.StageID)
                {
                    IEnumerator enumerator = st.GetComponentEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (index-- == 0)
                        {
                            return (IBaseComponent) enumerator.Current;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Apply per-instance pipeline configuration
        /// </summary>
        /// <param name="file">Path to the XML file with the configuration</param>
        /// <remarks>
        ///     Per-instance pipeline configuration uses the same XML format used
        ///     by the BizTalk Admin console that gets exported to binding files,
        ///     for example, in the &lt;SendPipelineData&gt; element
        /// </remarks>
        public void ApplyInstanceConfig(string file)
        {
            using (XmlReader reader = new XmlTextReader(file))
                ApplyInstanceConfig(reader);
        }

        /// <summary>
        ///     Apply per-instance pipeline configuration
        /// </summary>
        /// <param name="reader">XML reader with the configuration</param>
        /// <remarks>
        ///     Per-instance pipeline configuration uses the same XML format used
        ///     by the BizTalk Admin console that gets exported to binding files,
        ///     for example, in the &lt;SendPipelineData&gt; element
        /// </remarks>
        public void ApplyInstanceConfig(XmlReader reader)
        {
            Guid stageId = Guid.Empty;
            int index = 0;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "Stage")
                    {
                        stageId = new Guid(reader.GetAttribute("CategoryId"));
                        index = 0;
                    }
                    else if (reader.LocalName == "Component")
                    {
                        string name = reader.GetAttribute("Name");
                        reader.ReadToDescendant("Properties");
                        XmlReader propReader = reader.ReadSubtree();
                        propReader.Read();
                        ApplyComponentConfig(stageId, name, index, propReader);
                        index++;
                    }
                }
            }
        }

        //Load Document specification
        public IDocumentSpec LoadDocSpec(Type schemaType)
        {
            try
            {
                if (schemaType == null)
                    throw new ArgumentNullException("schemaType");
                if (!schemaType.IsSubclassOf(typeof(SchemaBase)))
                    throw new ArgumentException("Type does not represent a schema", "schemaType");

                string typename = schemaType.FullName;
                string assemblyName = schemaType.Assembly.FullName;
                DocumentSpec docSpec = new DocumentSpec(typename, assemblyName);

                return docSpec;
            }
            catch 
            {

                LogEngine.DirectEventViewerLog($"Error in pipeline component during load specs for assembly {schemaType.Assembly.FullName} check system GAC.",1);
                return null;

            }

        }

        //Load Document specification usinf asm name too
        public IDocumentSpec LoadDocSpec(string typeName, string assemblyName)
        {
            if (String.IsNullOrEmpty(typeName))
                throw new ArgumentNullException("typeName");
            if (String.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException("assemblyName");

            return new DocumentSpec(typeName, assemblyName);
        }

        #region Properties

        //
        // Properties
        //

        internal IPipeline Pipeline
        {
            get { return _pipeline; }
        }

        internal IPipelineContext Context
        {
            get { return _pipelineContext; }
        }

        /// <summary>
        ///     Gets or Set the thumbprint for the Group
        ///     Signing Certificate. Null by default
        /// </summary>
        public string GroupSigningCertificate
        {
            get { return _pipelineContext.GetGroupSigningCertificate(); }
            set
            {
                IConfigurePipelineContext ctxt = (IConfigurePipelineContext) _pipelineContext;
                ctxt.SetGroupSigningCertificate(value);
            }
        }

        #endregion // Properties

        #region Protected Methods

        //
        // Protected Methods
        //

        /// <summary>
        ///     Finds a stage in the pipeline
        /// </summary>
        /// <param name="stage">Stage definition</param>
        /// <returns>The stage, if found, or a new stage if necessary</returns>
        protected PStage FindStage(PpStage stage)
        {
            PStage theStage = null;
            foreach (PStage pstage in _pipeline.Stages)
            {
                if (pstage.Id == stage.StageID)
                {
                    theStage = pstage;
                    break;
                }
            }
            if (theStage == null)
            {
                theStage = new PStage(stage.StageName, stage.ExecuteMethod, stage.StageID, _pipeline);
                _pipeline.Stages.Add(theStage);
            }
            return theStage;
        }

        /// <summary>
        ///     Creates a new pipeline context for the execution
        /// </summary>
        /// <returns>The new pipeline context.</returns>
        protected IPipelineContext CreatePipelineContext()
        {
            return new PipelineContext();
        }

        #endregion // Protected Methods

        #region Private Methods

        //
        // Private Methods
        //

        /// <summary>
        ///     Gets the namespace#root name for a schema.
        ///     If the schema has multiple roots, all are returned.
        /// </summary>
        /// <param name="schemaType">Type of the schema</param>
        /// <returns>Roots of the schema</returns>
        private Type[] GetSchemaRoots(Type schemaType)
        {
            string root = GetSchemaRoot(schemaType);
            if (root != null)
            {
                return new[] {schemaType};
            }
            Type[] rts = schemaType.GetNestedTypes();
            return rts;
        }

        /// <summary>
        ///     Gets the root name (namespace#root) for a schema type
        /// </summary>
        /// <param name="schemaType">Type of the schema</param>
        /// <returns>Roots of the schema</returns>
        private string GetSchemaRoot(Type schemaType)
        {
            SchemaAttribute[] attrs = (SchemaAttribute[])
                schemaType.GetCustomAttributes(typeof(SchemaAttribute), true);
            if (attrs.Length > 0)
            {
                if (String.IsNullOrEmpty(attrs[0].TargetNamespace))
                    return attrs[0].RootElement;
                return string.Format("{0}#{1}", attrs[0].TargetNamespace, attrs[0].RootElement);
            }
            return null;
        }

        /// <summary>
        ///     Adds a document specification to the context
        /// </summary>
        /// <param name="docSpec">Specification to add</param>
        private void AddDocSpecToContext(IDocumentSpec docSpec)
        {
            IConfigurePipelineContext ctxt = (IConfigurePipelineContext) Context;
            ctxt.AddDocSpecByType(docSpec.DocType, docSpec);
            // Pipelines referencing local schemas in the same
            // assembly don't have use the assembly qualified name
            // of the schema when trying to find it.
            ctxt.AddDocSpecByName(docSpec.DocSpecStrongName, docSpec);
            ctxt.AddDocSpecByName(docSpec.DocSpecName, docSpec);
        }

        /// <summary>
        ///     Applies the loaded configuration to a component
        /// </summary>
        /// <param name="stageId">The stage the component is in</param>
        /// <param name="name">The component name</param>
        /// <param name="index">The index of the component within the pipeline</param>
        /// <param name="reader">The per-instance configuration</param>
        private void ApplyComponentConfig(Guid stageId, string name, int index, XmlReader reader)
        {
            PpStage stage = PpStage.LookupStage(stageId);
            IPersistPropertyBag component = GetComponent(stage, index)
                as IPersistPropertyBag;
            if (component != null)
            {
                String compName = component.GetType().FullName;
                if (compName != name)
                    throw new InvalidOperationException(String.Format(
                        "Component in stage '{0}', index {1} is '{2}', expected '{3}'",
                        stage.StageName, index, compName, name));

                IPropertyBag bag = new InstConfigPropertyBag(reader);
                component.Load(bag, 1);
            }
        }

        #endregion // Private Methods
    }
} // namespace BTSGBizTalkAddins