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
using System.IO;
using System.Threading.Tasks;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Serialization;

#endregion

namespace SnapGate.Framework.FileEvent
{
    /// <summary>
    ///     The file event.
    /// </summary>
    [EventContract("{D438C746-5E75-4D59-B595-8300138FB1EA}", "Write File",
        "Write the content in a file in a specific folder.", true)]
    public class FileEvent : IEventType
    {
        /// <summary>
        ///     Gets or sets the output directory.
        /// </summary>
        [EventPropertyContract("MacroFileName", "specify the file name to use")]
        public string MacroFileName { get; set; }

        /// <summary>
        ///     Gets or sets the output directory.
        /// </summary>
        [EventPropertyContract("OutputDirectory", "When the file has to be created")]
        public string OutputDirectory { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action event.
        /// </summary>
        public ActionEvent ActionEvent { get; set; }

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        [EventPropertyContract("DataContext", "Event Default Main Data")]
        public byte[] DataContext { get; set; }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="actionEvent">
        ///     The set event action event.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        [EventActionContract("{1FBD0C6E-1A49-4BEF-8876-33A21B23C933}", "Main action", "Main action description")]
        public async Task Execute(ActionEvent actionEvent, ActionContext context)
        {
            try
            {
                Trace.WriteLine("In FileEvent Event.");
                File.WriteAllBytes(OutputDirectory + GenerateFileName(MacroFileName),
                    DataContext == null ? new byte[0] : DataContext);
                DataContext = Serialization.ObjectToByteArray(true);
                actionEvent(this, context);
                return;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("FileEvent error > " + ex.Message);
                actionEvent(this, null);
                return;
            }
        }

        private string GenerateFileName(string fileNameMacro)
        {
            string fileName = "";
            fileName = fileNameMacro.Replace("%DATETIME%", DateTime.Now.ToString("yy-MM-dd-hh-mm-ss"))
                .Replace("%DATE%", DateTime.Now.ToString("yy-MM-dd"))
                .Replace("%GUID%", Guid.NewGuid().ToString());

            return fileName;
        }
    }
}