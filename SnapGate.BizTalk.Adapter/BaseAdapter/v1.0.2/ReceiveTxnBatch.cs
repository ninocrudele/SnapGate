// ReceiveTxnBatch.cs
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
using System.Transactions;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.BizTalk.TransportProxy.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace SnapGate.Framework.BizTalk.Adapter.Common
{
	//  Anything fails in this batch we abort the lot!
	public sealed class AbortOnFailureReceiveTxnBatch : TxnBatch
	{
		public delegate void TxnAborted ();

		private TxnAborted txnAborted;

        public AbortOnFailureReceiveTxnBatch(IBTTransportProxy transportProxy, ControlledTermination control, CommittableTransaction transaction, ManualResetEvent orderedEvent, TxnAborted txnAborted) : base(transportProxy, control, transaction, orderedEvent, false)
        { 
			this.txnAborted = txnAborted;
		}
        public AbortOnFailureReceiveTxnBatch(IBTTransportProxy transportProxy, ControlledTermination control, IDtcTransaction comTxn, CommittableTransaction transaction, ManualResetEvent orderedEvent, TxnAborted txnAborted)
            : base(transportProxy, control, comTxn, transaction, orderedEvent, false)
        {
            this.txnAborted = txnAborted;
        }
        protected override void StartBatchComplete (int hrBatchComplete)
        {
            if (this.HRStatus >= 0)
            {
                this.SetComplete();
            }
        }
        protected override void StartProcessFailures ()
		{
			this.SetAbort();
			if (this.txnAborted != null)
			{
				this.txnAborted();
			}
		}
	}

	//  Anything fails in this batch we abort the lot - even the case where we get an "S_FALSE" because the EPM has handled the error
	public sealed class AbortOnAllFailureReceiveTxnBatch : TxnBatch
	{
		public delegate void StopProcessing ();

		private StopProcessing stopProcessing;

        public AbortOnAllFailureReceiveTxnBatch(IBTTransportProxy transportProxy, ControlledTermination control, CommittableTransaction transaction, ManualResetEvent orderedEvent, StopProcessing stopProcessing) : base(transportProxy, control, transaction, orderedEvent, false)
        {
			this.stopProcessing = stopProcessing;
		}
		protected override void StartBatchComplete (int hrBatchComplete)
		{
            if (this.HRStatus != 0)
            {
                this.SetAbort();
                if (this.stopProcessing != null)
                {
                    this.stopProcessing();
                }
            }
            else
            {
                this.SetComplete();
            }
		}
	}

	//  Submit fails we MoveToSuspendQ
	public sealed class SingleMessageReceiveTxnBatch : TxnBatch
	{
        public SingleMessageReceiveTxnBatch(IBTTransportProxy transportProxy, ControlledTermination control, CommittableTransaction transaction, ManualResetEvent orderedEvent) : base(transportProxy, control, transaction, orderedEvent, true)
        { }
		protected override void StartProcessFailures ()
		{
			if (!this.OverallSuccess)
			{
				this.innerBatch = new AbortOnFailureReceiveTxnBatch(this.TransportProxy, this.control, this.comTxn, this.transaction, this.orderedEvent, null);
				this.innerBatchCount = 0;
			}
		}
		protected override void SubmitFailure (IBaseMessage message, Int32 hrStatus, object userData)
		{
			if (this.innerBatch != null)
			{
                try
                {
                    Stream originalStream = message.BodyPart.GetOriginalDataStream();
				    originalStream.Seek(0, SeekOrigin.Begin);
				    message.BodyPart.Data = originalStream;
                    this.innerBatch.MoveToSuspendQ(message);
					this.innerBatchCount++;
				}
				catch (Exception e)
				{
                    Trace.WriteLine("SingleMessageReceiveTxnBatch.SubmitFailure Exception: {0}", e.Message);
					this.innerBatch = null;
					this.SetAbort();
				}
			}
		}
        protected override void SubmitSuccess(IBaseMessage message, Int32 hrStatus, object userData)
        {
            this.SetComplete();
        }
		protected override void EndProcessFailures ()
		{
			if (this.innerBatch != null && this.innerBatchCount > 0)
			{
				try
				{
					this.innerBatch.Done();
					this.SetPendingWork();
				}
				catch (Exception)
				{
					this.SetAbort();
				}
			}
		}
		private Batch innerBatch;
		private int innerBatchCount;
	}
}

