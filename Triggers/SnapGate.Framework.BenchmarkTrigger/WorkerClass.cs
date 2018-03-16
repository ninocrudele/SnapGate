// Copyright 2016-2040 Nino Crudele
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Threading;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

namespace SnapGate.Framework.BenchmarkTrigger
{
    public class WorkerClass
    {
        public WorkerClass(int intervalTime, byte[] dataContext, ActionTrigger actionTrigger, ITriggerType triggerType,
            ActionContext actionContext)
        {
            this.IntervalTime = intervalTime;
            this.DataContext = dataContext;
            this.ActionContext = actionContext;
            this.ActionTrigger = actionTrigger;
            this.TriggerType = triggerType;
        }

        private int IntervalTime { get; set; }
        private byte[] DataContext { get; set; }
        private ITriggerType TriggerType { get; set; }
        private ActionContext ActionContext { get; set; }
        private ActionTrigger ActionTrigger { get; set; }

        private void WorkerMethod()
        {
            while (true)
            {
                SnapGate.Framework.EmbeddedTrigger.EmbeddedTrigger embeddedTrigger =
                    new EmbeddedTrigger.EmbeddedTrigger();
                embeddedTrigger.DataContext = DataContext;
                embeddedTrigger.Execute(ActionTrigger, ActionContext);
                Thread.Sleep(IntervalTime);
            }
        }


        public void Startjob(int numberOfThreads)
        {
            for (int i = 0; i < numberOfThreads; i++)
            {
                this.OnThread(WorkerMethod);
            }
        }
    }
}