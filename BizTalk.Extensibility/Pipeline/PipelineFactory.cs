// PipelineFactory.cs
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
using Microsoft.Test.BizTalk.PipelineObjects;
using PipelineHelper = Microsoft.Test.BizTalk.PipelineObjects.PipelineFactory;
using ReceivePipeline = Microsoft.BizTalk.PipelineOM.ReceivePipeline;
using SendPipeline = Microsoft.BizTalk.PipelineOM.SendPipeline;

#endregion

namespace SnapGate.BizTalk.Extensibility
{
    //Main clas to create pipelines
    public static class MainPipelineHelper
    {
        public static RWPipeline RetReceivePipeline(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("Pipeline type null, deploy Pipeline in BizTalk BizTalkMgmtDb database");

            if (!type.IsSubclassOf(typeof(ReceivePipeline)))
                throw new InvalidOperationException("Type must specify a Receive Pipeline");

            PipelineHelper helper = new PipelineHelper();
            IPipeline pipeline = helper.CreatePipelineFromType(type);
            return new RWPipeline(pipeline);
        }

        public static SWPipeline RetSendPipeline(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!type.IsSubclassOf(typeof(SendPipeline)))
                throw new InvalidOperationException("Type must specify a Send Pipeline");

            PipelineHelper helper = new PipelineHelper();
            IPipeline pipeline = helper.CreatePipelineFromType(type);
            return new SWPipeline(pipeline);
        }
    }
}