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
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Messaging;
using SnapGate.Framework.Log;
using StackExchange.Redis;

#endregion

namespace SnapGate.Framework.Dcp.Redis
{
    [EventsOffRampContract("{A51FA36B-7778-47A1-B6DF-5CEC4B8F36B1}", "EventUpStream", "Redis EventUpStream")]
    class OffRampStream : IOffRampStream
    {
        private ISubscriber subscriber;

        public bool CreateOffRampStream()
        {
            try
            {
                ConnectionMultiplexer redis =
                    ConnectionMultiplexer.Connect(ConfigurationBag.Configuration.RedisConnectionString);
                subscriber = redis.GetSubscriber();
                return true;
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
                return false;
            }
        }

        public void SendMessage(BubblingObject message)
        {
            try
            {
                byte[] byteArrayBytes = BubblingObject.SerializeMessage(message);
                subscriber.Publish("*", byteArrayBytes);
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