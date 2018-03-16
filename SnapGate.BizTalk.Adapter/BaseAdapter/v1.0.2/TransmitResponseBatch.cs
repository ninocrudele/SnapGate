// TransmitResponseBatch.cs
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
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.BizTalk.TransportProxy.Interop;
using Microsoft.BizTalk.Message.Interop;

/// <summary>
/// Summary description for Class1
/// </summary>
namespace SnapGate.Framework.BizTalk.Adapter.Common
{
    /// <summary>
    /// This class encapsulates the typical behavior we want in a Transmit Adapter running asynchronously.
    /// In summary our policy is:
    /// (1) on a resubmit failure Move to the next transport
    /// (2) on a move to next transport failure move to the suspend queue
    /// Otherwise:
    /// TODO: we should use SetErrorInfo on the transportProxy to log the error appropriately 
    /// </summary>
    public sealed class TransmitResponseBatch : Batch
    {
        public delegate void AllWorkDoneDelegate();

        private AllWorkDoneDelegate allWorkDoneDelegate;

        public TransmitResponseBatch(IBTTransportProxy transportProxy, AllWorkDoneDelegate allWorkDoneDelegate)
            : base(transportProxy, true)
        {
            this.allWorkDoneDelegate = allWorkDoneDelegate;
        }

        public override sealed void SubmitResponseMessage(IBaseMessage solicitDocSent, IBaseMessage responseDocToSubmit)
        {
            IBaseMessagePart bodyPart = responseDocToSubmit.BodyPart;
            if (bodyPart == null)
                throw new InvalidOperationException("The message does not contain body part");

            Stream stream = bodyPart.GetOriginalDataStream();

            if (stream == null || stream.CanSeek == false)
                throw new InvalidOperationException("Message body stream is null or it is not seekable");
            
            base.SubmitResponseMessage(solicitDocSent, responseDocToSubmit, solicitDocSent);
        }

        // This method is typically used during process shutdown
        public void Resubmit(IBaseMessage[] msgs, DateTime timeStamp)
        {
            foreach (IBaseMessage message in msgs)
                base.Resubmit(message, timeStamp);
        }

        public void Resubmit(IBaseMessage msg, bool preserveRetryCount, object userData)
        {
            SystemMessageContext context = new SystemMessageContext(msg.Context);

            if (preserveRetryCount)
            {
                UpdateProperty[] updates = new UpdateProperty[1];
                updates[0] = new UpdateProperty();
                updates[0].Name = retryCountProp.Name.Name;
                updates[0].NameSpace = retryCountProp.Name.Namespace;
                updates[0].Value = context.RetryCount++;

                context.UpdateProperties(updates);

                // If preserveRetryCount is true, ignore RetryInterval
                // Request the redelivery immediately!!
                base.Resubmit(msg, DateTime.Now, userData);
            }
            else
            {
                // This is retry in case of error/failure (i.e. normal retry)
                if (context.RetryCount > 0)
                {
                    DateTime retryAt = DateTime.Now.AddMinutes(context.RetryInterval);
                    base.Resubmit(msg, retryAt, userData);
                }
                else
                {
                    base.MoveToNextTransport(msg, userData);
                }
            }
        }

        protected override void StartBatchComplete(int hrBatchComplete)
        {
            this.batchFailed = (this.HRStatus < 0);
        }

        protected override void StartProcessFailures()
        {
            if (this.batchFailed)
            {
                // Retry should happen outside the transaction scope
                this.batch = new TransmitResponseBatch(this.TransportProxy, this.allWorkDoneDelegate);
                this.allWorkDoneDelegate = null;
            }
        }

        protected override void EndProcessFailures()
        {
            if (this.batch != null)
            {
                if (!this.batch.IsEmpty)
                {
                    this.batch.Done(null);
                }
                else
                {
                    // If suspend or delete fails, then there is nothing adapter can do!
                    this.batch.Dispose();
                }
            }

        }

        protected override void EndBatchComplete()
        {
            if (null != this.allWorkDoneDelegate)
                this.allWorkDoneDelegate();
        }

        // This is for submit-response
        protected override void SubmitSuccess(IBaseMessage message, Int32 hrStatus, object userData)
        {
            if (this.batchFailed)
            {
                // Previous submit operation might have moved the stream position
                // Seek the stream position back to zero before submitting again!
                IBaseMessage solicit = userData as IBaseMessage;
                if (solicit == null)
                    throw new InvalidOperationException("Response message does not have corresponding request message");

                IBaseMessagePart responseBodyPart = message.BodyPart;

                if (responseBodyPart != null )
                {
                    Stream stream = responseBodyPart.GetOriginalDataStream();
                    stream.Position = 0;
                }
                this.batch.SubmitResponseMessage(solicit, message);
            }
        }

        protected override void SubmitFailure(IBaseMessage message, Int32 hrStatus, object userData)
        {
            // If response cannot be submitted, then Resubmit the original message?
            // this.batch.Resubmit(message, false, null);
            this.batch.MoveToSuspendQ(message);
        }

        protected override void DeleteSuccess(IBaseMessage message, Int32 hrStatus, object userData)
        {
            if (this.batchFailed)
            {
                this.batch.DeleteMessage(message);
            }
        }

        // No action required when delete fails!

        protected override void ResubmitSuccess(IBaseMessage message, Int32 hrStatus, object userData)
        {
            if (this.batchFailed)
            {
                SystemMessageContext context = new SystemMessageContext(message.Context);
                DateTime dt = DateTime.Now.AddMinutes(context.RetryInterval);
                this.batch.Resubmit(message, dt);
            }
        }

        protected override void ResubmitFailure(IBaseMessage message, Int32 hrStatus, object userData)
        {
            this.batch.MoveToNextTransport(message);
        }

        protected override void MoveToNextTransportSuccess(IBaseMessage message, Int32 hrStatus, object userData)
        {
            if (this.batchFailed)
            {
                this.batch.MoveToNextTransport(message);
            }
        }

        protected override void MoveToNextTransportFailure(IBaseMessage message, Int32 hrStatus, object userData)
        {
            this.batch.MoveToSuspendQ(message);
        }

        protected override void MoveToSuspendQSuccess(IBaseMessage message, Int32 hrStatus, object userData)
        {
            if (this.batchFailed)
            {
                this.batch.MoveToSuspendQ(message);
            }
        }

        // Nothing can be done if suspend fails

        private TransmitResponseBatch batch;
        private bool batchFailed = false;
        private static BTS.RetryCount retryCountProp = new BTS.RetryCount();
    }
}
