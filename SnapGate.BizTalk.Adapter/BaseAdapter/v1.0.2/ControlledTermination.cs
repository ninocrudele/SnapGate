// ControlledTermination.cs
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

namespace SnapGate.Framework.BizTalk.Adapter.Common
{
    public class ControlledTermination : IDisposable
    {
        private AutoResetEvent e = new AutoResetEvent(false);
        private int activityCount = 0;
        private bool terminate = false;

        //  to be called at the start of the activity
        //  returns false if terminate has been called
        public bool Enter ()
        {
            lock (this)
            {
                if (true == this.terminate)
                {
                    return false;
                }

                this.activityCount++;
            }
            return true;
        }

        //  to be called at the end of the activity
        public void Leave ()
        {
            lock (this)
            {
                this.activityCount--;

                // Set the event only if Terminate() is called
                if (this.activityCount == 0 && this.terminate)
                    this.e.Set();
            }
        }

        //  this method blocks waiting for any activity to complete
        public void Terminate ()
        {
            bool result;

            lock (this)
            {
                this.terminate = true;
                result = (this.activityCount == 0);
            }

            // If activity count was not zero, wait for pending activities
            if (!result)
            {
                this.e.WaitOne();
            }
        }

        public bool TerminateCalled
        {
            get
            { 
                lock (this)
                {
                    return this.terminate;
                }
            }
        }

        public void Dispose()
        {
            ((IDisposable)this.e).Dispose();
        }
    }
}