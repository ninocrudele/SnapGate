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

#region Usings

using System.Diagnostics;

#endregion

namespace SnapGate.Framework.PerfCounters
{
    /// <summary>
    ///     The performance counters.
    /// </summary>
    public class PerformanceCounters
    {
        /// <summary>
        ///     Counter for counting total number of operations
        /// </summary>
        private readonly PerformanceCounter counterTotalTriggerCalls;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PerformanceCounters" /> class.
        ///     Creates a new performance counter category "MyCategory" if it does not already exists and adds some counters to it.
        /// </summary>
        public PerformanceCounters()
        {
            if (PerformanceCounterCategory.Exists("SnapGate"))
            {
                PerformanceCounterCategory.Delete("SnapGate");
            }

            var counters = new CounterCreationDataCollection();
            // Counter for counting totals: PerformanceCounterType.NumberOfItems32
            var totalTriggerCalls = new CounterCreationData
            {
                CounterName = "# trigger executed",
                CounterHelp = "Total number of triggers executed per second",
                CounterType = PerformanceCounterType.RateOfCountsPerSecond64
            };
            counters.Add(totalTriggerCalls);

            // create new category with the counters above

            //PerformanceCounterCategory.Create("SnapGate", "SnapGate counters", counters);


            // create counters to work with
            counterTotalTriggerCalls = new PerformanceCounter();
            counterTotalTriggerCalls.CategoryName = "SnapGate";
            counterTotalTriggerCalls.CounterName = "# trigger executed";
            counterTotalTriggerCalls.MachineName = ".";
            counterTotalTriggerCalls.ReadOnly = false;
            counterTotalTriggerCalls.RawValue = 0;
        }

        /// <summary>
        ///     Increments counters.
        /// </summary>
        public void DoSomeProcessing()
        {
            // simply increment the counters
            counterTotalTriggerCalls.Increment();
        }
    }
}