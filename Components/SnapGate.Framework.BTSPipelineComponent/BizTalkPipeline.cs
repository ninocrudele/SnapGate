// BizTalkPipeline.cs
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
using System.Reflection;
using SnapGate.BizTalk.Extensibility;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Components;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.BTSPipelineComponent
{
    [ComponentContract("{F60C8A3B-0ABD-4595-BCFD-7A2B6DE46EC6}", "BizTalk Pipeline Executor",
         "Execute a BizTalk Pipeline")]
    public class BizTalkPipeline : IChainComponentType
    {
        [ComponentPropertyContract("AssemblyFile", "Pipeline assembly file")]
        public string AssemblyFile { get; set; }

        [ComponentPropertyContract("PipelineTypeName", "Pipeline type name")]
        public string PipelineTypeName { get; set; }

        [ComponentPropertyContract("PipelinePathFile", "Pipeline path file")]
        public string PipelinePathFile { get; set; }

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        /// <value>
        ///     The data context.
        /// </value>
        [ComponentPropertyContract("DataContext", "Event Default Main Data")]
        public byte[] DataContext { get; set; }

        [ComponentActionContract("{0CC64C13-5BCC-440B-B858-1164A76E4CA9}", "Main action", "Main action description")]
        public byte[] Execute()
        {
            try
            {
                string content = EncodingDecoding.EncodingBytes2String(DataContext);
                byte[] contentBack = BizTalkPipelines.ExecutePipeline(AssemblyFile, AssemblyFile, PipelineTypeName,
                    content, PipelinePathFile);
                return contentBack;
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