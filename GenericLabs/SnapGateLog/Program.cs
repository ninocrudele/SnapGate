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
using System.Diagnostics;
using System.Threading;

namespace SnapGateLog
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write($"Trace.Writeline Test started");
            long minimum = 0;
            long numberOfMessages = 0;
            long avgNumberOfMessages = 0;


            ;


            int numcalls = 0;
            long totalMilliseconds = 0;

            while (true)
            {
                numcalls++;
                var watch = Stopwatch.StartNew();

                // the code that you want to measure comes here
                for (int i = 0; i < 5000; i++)
                {
                    Trace.WriteLine($"ActionTriggerReceived1 with DataContext = null " +
                                    $"ActionTriggerReceived bubblingObject.SenderChannelId " +
                                    $"bubblingObject.SenderPointId " +
                                    $"bubblingObject.DestinationChannelId " +
                                    $" bubblingObject.DestinationPointId  " +
                                    $"bubblingObject.MessageType " +
                                    $"bubblingObject.Persisting " +
                                    $"bubblingObject.MessageId " +
                                    $"bubblingObject.Name " +
                                    $"bubblingObject.IdConfiguration " +
                                    $"bubblingObject.IdComponent ");


                    //LogEngine.WriteLog("ConfigurationBag.EngineName",
                    //    $"ActionTriggerReceived1 with DataContext = null " +
                    //    $"ActionTriggerReceived bubblingObject.SenderChannelId " +
                    //    $"bubblingObject.SenderPointId " +
                    //    $"bubblingObject.DestinationChannelId " +
                    //    $" bubblingObject.DestinationPointId  " +
                    //    $"bubblingObject.MessageType " +
                    //    $"bubblingObject.Persisting " +
                    //    $"bubblingObject.MessageId " +
                    //    $"bubblingObject.Name " +
                    //    $"bubblingObject.IdConfiguration " +
                    //    $"bubblingObject.IdComponent ",
                    //    Constant.LogLevelError,
                    //    Constant.TaskCategoriesConsole,
                    //    null,
                    //    Constant.LogLevelInformation);
                }

                watch.Stop();
                totalMilliseconds = totalMilliseconds + watch.ElapsedTicks;

                Console.WriteLine(
                    $"AVG Millisecond totalMilliseconds {totalMilliseconds} /calls {numcalls} : {totalMilliseconds / numcalls}");
                Thread.Sleep(1000);
                Console.Clear();
                watch.Restart();
            }
        }
    }
}