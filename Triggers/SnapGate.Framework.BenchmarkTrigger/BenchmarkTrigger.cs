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

using System;
using System.IO;
using System.Threading;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

namespace SnapGate.Framework.BenchmarkTrigger
{
    /// <summary>
    ///     The file trigger.
    /// </summary>
    [TriggerContract("{66F457F8-C754-4FBA-B25B-DC88F5F170AE}", "BenchmarkTrigger",
        "Get the content from file in a specific directory and execute a benchmark test.", false, true, false)]
    public class BenchmarkTrigger : ITriggerType
    {
        /// <summary>
        ///     File content to use for the benchmark..
        /// </summary>
        [TriggerPropertyContract("PathFilename", "File content to use for the benchmark.")]
        public string PathFilename { get; set; }

        /// <summary>
        ///     Polling time in the thread.
        /// </summary>
        [TriggerPropertyContract("ThreadPollingTime", "Polling time in the thread.")]
        public int ThreadPollingTime { get; set; }

        /// <summary>
        ///     Number of threads.
        /// </summary>
        [TriggerPropertyContract("ThreadsNumber", "Number of threads.")]
        public int ThreadsNumber { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action trigger.
        /// </summary>
        public ActionTrigger ActionTrigger { get; set; }

        /// <summary>
        ///     If must be syncronous
        /// </summary>
        public bool Syncronous { get; set; }

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        [TriggerPropertyContract("DataContext", "Trigger Default Main Data")]
        public byte[] DataContext { get; set; }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="actionTrigger">
        ///     The set event action trigger.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        [TriggerActionContract("{75F69C1F-66C1-4251-9F90-35A743EDF22B}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                var data = File.ReadAllBytes(PathFilename);
                DataContext = data;
                var workerClass = new WorkerClass(ThreadPollingTime, DataContext, actionTrigger, this, context);
                workerClass.Startjob(ThreadsNumber);
                Thread.Sleep(Timeout.Infinite);
                return null;
            }
            catch (Exception)
            {
                actionTrigger(this, null);
                return null;
            }
        }
    }
}