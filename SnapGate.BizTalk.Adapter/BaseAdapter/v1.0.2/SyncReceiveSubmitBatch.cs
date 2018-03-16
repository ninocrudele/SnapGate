// SyncReceiveSubmitBatch.cs
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
using System.Threading;
using System.Collections.Generic;
using Microsoft.BizTalk.TransportProxy.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace SnapGate.Framework.BizTalk.Adapter.Common
{
    public class SyncReceiveSubmitBatch : ReceiveBatch
    {
        private ManualResetEvent workDone;
        private bool overallSuccess = false;
        private ControlledTermination control;

        public SyncReceiveSubmitBatch(IBTTransportProxy transportProxy, ControlledTermination control, int depth)
            : this(transportProxy, control, new ManualResetEvent(false), depth) { }

        private SyncReceiveSubmitBatch(IBTTransportProxy transportProxy, ControlledTermination control,
                                        ManualResetEvent submitComplete, int depth)
            : base(transportProxy, control, submitComplete, depth)
        {
            this.control = control;
            this.workDone = submitComplete;
            base.ReceiveBatchComplete += new ReceiveBatchCompleteHandler(OnBatchComplete);
        }

        private void OnBatchComplete(bool overallSuccess)
        {
            this.overallSuccess = overallSuccess;
        }

        public override void Done()
        {
            bool needToLeave = control.Enter();

            try
            {
                base.Done();
            }
            catch
            {
                if (needToLeave)
                    control.Leave();

                throw;
            }
        }

        public bool Wait()
        {
            this.workDone.WaitOne();

            return this.overallSuccess;
        }
    }
}