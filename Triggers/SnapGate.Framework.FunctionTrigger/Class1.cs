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

using System;
using System.Diagnostics;
using System.Threading;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace SnapGate.Framework.DynamicRESTTrigger
{
    /// <summary>
    ///     The file trigger.
    /// </summary>
    [TriggerContract("{20CEE583-B389-4BF3-AA4C-71E991B0F945}", "PageOneMessageTrigger",
        "PageOneMessageTrigger is the OMS service to send mails", false, true, false)]
    public class DynamicRESTTrigger : ITriggerType
    {
        //questo puoi metterlo in interfaccia
        public delegate byte[] SetGetDataTrigger(
            ActionTrigger actionTrigger, ActionContext context);

        public SetGetDataTrigger setGetDataTrigger;

        /// <summary>
        ///     WebApiEndPoint used by the service
        /// </summary>
        [TriggerPropertyContract("WebApiEndPoint", "WebApiEndPoint used by the service")]
        public string WebApiEndPoint { get; set; }


        public string SupportBag { get; set; }

        [TriggerPropertyContract("Syncronous", "Trigger Syncronous")]
        public bool Syncronous { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action trigger.
        /// </summary>
        public ActionTrigger ActionTrigger { get; set; }

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
        [TriggerActionContract("{62DD5BB1-C27B-4341-A277-FE7023775AC3}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                //setGetDataTrigger = GetDataTrigger;
                //DynamicRESTService.StartService(WebApiEndPoint);
                string guid = string.Empty;
                string guidBack = string.Empty;
                while (true)
                {
                    guid = Guid.NewGuid().ToString();
                    Console.WriteLine($"TRGT {guid} - {DateTime.UtcNow.Second + ":" + DateTime.UtcNow.Millisecond}");
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    DataContext = EncodingDecoding.EncodingString2Bytes(guid);
                    actionTrigger(this, context);

                    stopwatch.Stop();
                    guidBack = EncodingDecoding.EncodingBytes2String(DataContext);
                    Console.WriteLine(
                        $"EVTT {guidBack} - {DateTime.UtcNow.Second + ":" + DateTime.UtcNow.Millisecond}");

                    long elapsed_time = stopwatch.ElapsedMilliseconds;
                    Console.WriteLine($"Response in {elapsed_time}");
                    Console.WriteLine($"0.5 second waitining...");
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                DataContext = EncodingDecoding.EncodingString2Bytes(ex.Message);
                actionTrigger(this, context);
                ActionTrigger = actionTrigger;
                Context = context;
                return EncodingDecoding.EncodingString2Bytes(ex.Message);
            }
        }
    }
}