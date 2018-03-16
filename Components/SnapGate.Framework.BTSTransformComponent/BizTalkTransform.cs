// BizTalkTransform.cs
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
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Components;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.BTSTransformComponent
{
    [ComponentContract("{EFA41557-6AE0-4880-B8B2-EE3DF2C1E48E}", "BizTalk Transform Executor",
         "Execute a BizTalk Transform")]
    public class BizTalkTransform : IChainComponentType
    {
        [ComponentPropertyContract("AssemblyFile", "Transform assembly file")]
        public string AssemblyFile { get; set; }

        [ComponentPropertyContract("TransformTypeName", "Transform type name")]
        public string TransformTypeName { get; set; }

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        /// <value>
        ///     The data context.
        /// </value>
        [ComponentPropertyContract("DataContext", "Event Default Main Data")]
        public byte[] DataContext { get; set; }

        [ComponentActionContract("{C256542A-9248-4E37-A2C4-53AA801A82DC}", "Main action", "Main action description")]
        public byte[] Execute()
        {
            try
            {
                Assembly asm = Assembly.LoadFrom(AssemblyFile);

                Type t = asm.GetType(TransformTypeName);
                if (t == null)
                {
                    LogEngine.WriteLog(ConfigurationBag.EngineName,
                        $"Error in {MethodBase.GetCurrentMethod().Name} - {TransformTypeName} not found in assembly {AssemblyFile}",
                        Constant.LogLevelError,
                        Constant.TaskCategoriesError,
                        null,
                        Constant.LogLevelError);
                }

                object assemblyTransform = asm.CreateInstance(TransformTypeName);


                PropertyInfo pi = assemblyTransform.GetType()
                    .GetProperty("XmlContent", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
                object viewData = pi.GetValue(assemblyTransform, null);
                string tmpViewData = viewData != null ? viewData.ToString() : string.Empty;
                string dataXslt = tmpViewData.Replace("utf-16", "utf-8");

                XslCompiledTransform proc = new XslCompiledTransform();

                using (StringReader sr = new StringReader(dataXslt))
                {
                    using (XmlReader xr = XmlReader.Create(sr))
                    {
                        proc.Load(xr);
                    }
                }

                string resultXML;

                string xmlData = EncodingDecoding.EncodingBytes2String(DataContext);
                using (StringReader sr = new StringReader(xmlData))
                {
                    using (XmlReader xr = XmlReader.Create(sr))
                    {
                        using (StringWriter sw = new StringWriter())
                        {
                            proc.Transform(xr, null, sw);
                            resultXML = sw.ToString();
                            return EncodingDecoding.EncodingString2Bytes(resultXML);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);

                return null;
            }
        }
    }
}