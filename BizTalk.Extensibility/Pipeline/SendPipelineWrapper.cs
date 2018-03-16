// SendPipelineWrapper.cs
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
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Test.BizTalk.PipelineObjects;

#endregion

namespace SnapGate.BizTalk.Extensibility
{
    /// <summary>
    ///     Wrapper around a send pipeline you can execute
    /// </summary>
    public class SWPipeline : BWPipeline
    {
        internal SWPipeline(IPipeline pipeline)
            : base(pipeline, false)
        {
            FindStage(PpStage.PreAssembleStage);
            FindStage(PpStage.AssembleStage);
            FindStage(PpStage.EncodeStage);
        }


        /// <summary>
        ///     Execute the send pipeline
        /// </summary>
        /// <param name="inputMessages">Set of input messages to feed to the pipeline</param>
        /// <returns>The output message</returns>
        public IBaseMessage Execute(MessageCollection inputMessages)
        {
            if (inputMessages == null)
                throw new ArgumentNullException("inputMessages");
            if (inputMessages.Count <= 0)
                throw new ArgumentException("Must provide at least one input message", "inputMessages");

            foreach (IBaseMessage inputMessage in inputMessages)
            {
                Pipeline.InputMessages.Add(inputMessage);
            }

            MessageCollection output = new MessageCollection();
            Pipeline.Execute(Context);

            IBaseMessage om = Pipeline.GetNextOutputMessage(Context);
            return om;
        }

        /// <summary>
        ///     Executes the send pipeline with all messages
        ///     provided as inputs
        /// </summary>
        /// <param name="inputMessages">One or more input messages to the pipeline</param>
        /// <returns>The single output message</returns>
        public IBaseMessage Execute(params IBaseMessage[] inputMessages)
        {
            MessageCollection inputs = new MessageCollection();
            inputs.AddRange(inputMessages);
            return Execute(inputs);
        }
    } // class SendPipelineWrapper
} // namespace BTSGBizTalkAddins