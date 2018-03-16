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
using System.Reflection;
using System.Threading;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Messaging;
using SnapGate.Framework.Log;
using StackExchange.Redis;

#endregion

namespace SnapGate.Framework.Dcp.Redis
{
    [EventsOnRampContract("{377B04BD-C80C-4AC5-BC70-C5CC571B2BDC}", "EventsDownStream", "Redis EventsDownStream")]
    public class OnRampStream : IOnRampStream
    {
        public void Run(SetEventOnRampMessageReceived setEventOnRampMessageReceived)
        {
            try
            {
                var myNewThread = new Thread(() => StartRedisListener(setEventOnRampMessageReceived));
                myNewThread.Start();
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesEventHubs,
                    ex,
                    Constant.LogLevelError);
            }
        }

        public void StartRedisListener(SetEventOnRampMessageReceived setEventOnRampMessageReceived)
        {
            try
            {
                ConnectionMultiplexer redis =
                    ConnectionMultiplexer.Connect(ConfigurationBag.Configuration.RedisConnectionString);

                ISubscriber sub = redis.GetSubscriber();

                sub.Subscribe("*", (channel, message) =>
                {
                    byte[] byteArray = message;
                    BubblingObject bubblingObject = BubblingObject.DeserializeMessage(byteArray);
                    setEventOnRampMessageReceived(bubblingObject);
                });
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesEventHubs,
                    ex,
                    Constant.LogLevelError);
            }
        }
    }
}