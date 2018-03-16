// TxnBatch.cs
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
using System.Runtime.InteropServices;
using Microsoft.BizTalk.TransportProxy.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace SnapGate.Framework.BizTalk.Adapter.Common
{
	public class TxnBatch : Batch
	{
        public TxnBatch(IBTTransportProxy transportProxy, ControlledTermination control, CommittableTransaction transaction, ManualResetEvent orderedEvent, bool makeSuccessCall) : base(transportProxy, makeSuccessCall)
        {
			this.control = control;

            this.comTxn = TransactionInterop.GetDtcTransaction(transaction);

            //  the System.Transactions transaction - must be the original transaction - only that can be used to commit
            this.transaction = transaction;

			this.orderedEvent = orderedEvent;
		}
        public TxnBatch(IBTTransportProxy transportProxy, ControlledTermination control, IDtcTransaction comTxn, CommittableTransaction transaction, ManualResetEvent orderedEvent, bool makeSuccessCall) : base(transportProxy, makeSuccessCall)
        {
            this.control = control;
            this.comTxn = comTxn;
            this.transaction = transaction;
            this.orderedEvent = orderedEvent;
        }
        public override void Done ()
		{
            this.CommitConfirm = base.Done(this.comTxn);
		}
		protected override void EndBatchComplete ()
		{
			if (this.pendingWork)
			{
				return;
			}
			try
			{
				if (this.needToAbort)
				{
                    this.transaction.Rollback();

                    this.CommitConfirm.DTCCommitConfirm(this.comTxn, false); 
				}
				else
				{
                    this.transaction.Commit();

                    this.CommitConfirm.DTCCommitConfirm(this.comTxn, true); 
				}
			}
			catch
			{
				try
				{
					this.CommitConfirm.DTCCommitConfirm(this.comTxn, false); 
				}
				catch
				{
				}
			}
			//  note the pending work check at the top of this function removes the need to check a needToLeave flag
			this.control.Leave();

			if (this.orderedEvent != null)
				this.orderedEvent.Set();
		}
        protected void SetAbort()
        {
            this.needToAbort = true;
        }
        protected void SetComplete()
        {
            this.needToAbort = false;
        }
        protected void SetPendingWork()
		{
			this.pendingWork = true;
		}
		protected IBTDTCCommitConfirm CommitConfirm
		{
			set
			{
				this.commitConfirm = value;
				this.commitConfirmEvent.Set();
			}
			get
			{
				this.commitConfirmEvent.WaitOne();
				return this.commitConfirm;
			}
		}
        protected IDtcTransaction comTxn;
        protected CommittableTransaction transaction;
        protected ControlledTermination control;
		protected IBTDTCCommitConfirm commitConfirm = null;
		protected ManualResetEvent orderedEvent;
		private ManualResetEvent commitConfirmEvent = new ManualResetEvent(false);
		private bool needToAbort = true;
		private bool pendingWork = false;
	}
}
