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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Messaging;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.Engine.OnRamp
{
    /// <summary>
    ///     The on ramp engine.
    /// </summary>
    public sealed class OnRampEngineQueue : LockSlimQueueEngine<BubblingObject>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OnRampEngineQueue" /> class.
        /// </summary>
        /// <param name="capLimit">
        ///     The cap limit.
        /// </param>
        /// <param name="timeLimit">
        ///     The time limit.
        /// </param>
        public OnRampEngineQueue(int capLimit, int timeLimit)
        {
            CapLimit = capLimit;
            TimeLimit = timeLimit;
            InitTimer();
        }
    }

    /// <summary>
    ///     The on ramp engine receiving.
    /// </summary>
    public class OnRampEngineReceiving
    {
        /// <summary>
        ///     The parameters ret.
        /// </summary>
        private static readonly object[] ParametersRet = {null};

        private static IOnRampStream OnRampStream;

        /// <summary>
        ///     Create the internal queue
        /// </summary>
        private readonly OnRampEngineQueue _onRampEngineQueue;

        /// <summary>
        ///     Delegate used to fire the event to enqueue the message.
        /// </summary>
        private SetEventOnRampMessageReceived receiveMessageOnRampDelegate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OnRampEngineReceiving" /> class.
        /// </summary>
        public OnRampEngineReceiving()
        {
            _onRampEngineQueue = new OnRampEngineQueue(
                ConfigurationBag.Configuration.ThrottlingOnRampIncomingRateNumber,
                ConfigurationBag.Configuration.ThrottlingOnRampIncomingRateSeconds);
            _onRampEngineQueue.OnPublish += OnRampEngineQueueOnPublish;
        }

        /// <summary>
        ///     Initialize the onramp engine.
        /// </summary>
        /// <param name="onRampPatternComponent">
        ///     The off ramp pattern component.
        /// </param>
        public void Init(string onRampPatternComponent)
        {
            // Delegate event for ingestor where ReceiveMessageOnRamp is the event
            receiveMessageOnRampDelegate = ReceiveMessageOnRamp;

            LogEngine.WriteLog(
                ConfigurationBag.EngineName,
                "Start On Ramp engine.",
                Constant.LogLevelError,
                Constant.TaskCategoriesError,
                null,
                Constant.LogLevelInformation);

            // Inizialize the MSPC

            // Load event up stream external component
            var eventsUpStreamComponent = Path.Combine(
                ConfigurationBag.Configuration.DirectoryOperativeRootExeName,
                ConfigurationBag.Configuration.EventsStreamComponent);

            // Create the reflection method cached 
            var assembly = Assembly.LoadFrom(eventsUpStreamComponent);

            // Main class loggingCreateOnRamptream
            var assemblyClass = (from t in assembly.GetTypes()
                let attributes = t.GetCustomAttributes(typeof(EventsOnRampContract), true)
                where t.IsClass && attributes != null && attributes.Length > 0
                select t).First();


            OnRampStream = Activator.CreateInstance(assemblyClass) as IOnRampStream;
            OnRampStream.Run(receiveMessageOnRampDelegate);
        }

        /// <summary>
        ///     Send the message to the engine message.
        /// </summary>
        /// <param name="objects">
        ///     The objects.
        /// </param>
        private static void OnRampEngineQueueOnPublish(List<BubblingObject> objects)
        {
            foreach (var message in objects)
            {
                // Sent message to the MSPC
                MessageIngestor.IngestMessagge(message);
            }
        }

        /// <summary>
        ///     Event fired by the On Ramp engine.
        /// </summary>
        /// <param name="message">
        ///     The message.
        /// </param>
        private void ReceiveMessageOnRamp(BubblingObject message)
        {
            _onRampEngineQueue.Enqueue(message);
        }
    }
}