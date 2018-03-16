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

using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Log;

#endregion

namespace SnapGate.Framework.Log.EventHubs
{
    /// <summary>
    ///     The log engine.
    /// </summary>
    [LogContract("AEC1AF21-2131-475D-AEFE-DDCA2D835466", "LogEngine", "Event Hubs Log System")]
    public class LogEngine : ILogEngine
    {
        /// <summary>
        ///     Initialize log.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool InitLog()
        {
            return LogEventUpStream.CreateEventUpStream();
        }

        /// <summary>
        ///     The write log.
        /// </summary>
        /// <param name="logMessage">
        ///     The log message.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool WriteLog(LogMessage logMessage)
        {
            return LogEventUpStream.SendMessage(logMessage);
        }

        public void Flush()
        {
            //NOP
        }
    }
}