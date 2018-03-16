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
using System.IO;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Log;

#endregion

namespace SnapGate.Framework.Log.File
{
    /// <summary>
    ///     The log engine, simple version.
    /// </summary>
    [LogContract("{4DACE829-1462-4A3D-ACC9-1EE41B3C2D53}", "LogEngine", "File Log System")]
    public class LogEngine : ILogEngine
    {
        StreamWriter logFile;
        private string PathFile = "";

        /// <summary>
        ///     Initialize log.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool InitLog()
        {
            Directory.CreateDirectory(ConfigurationBag.DirectoryLog());
            PathFile = Path.Combine(ConfigurationBag.DirectoryLog(),
                $"{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Year}-{Guid.NewGuid()}.txt");
            logFile = System.IO.File.AppendText(PathFile);
            return true;
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
            lock (logFile)
            {
                logFile.WriteLine($"{DateTime.Now} - {logMessage.Message}");
                return true;
            }
        }

        public void Flush()
        {
            logFile.Flush();
        }
    }
}