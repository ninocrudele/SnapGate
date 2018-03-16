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

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Roslyn.Compilers;
using Roslyn.Scripting.CSharp;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;

#endregion

namespace SnapGate.Framework.CSharpEvent
{
    /// <summary>
    ///     The c sharp event.
    /// </summary>
    [EventContract("{A54EDF55-52E9-4D03-B14B-F5D438AF43F1}", "Execute a CSharp Event", "Execute a CSharp Event", true)]
    public class CSharpEvent : IEventType
    {
        /// <summary>
        ///     Gets or sets the script.
        /// </summary>
        [EventPropertyContract("Script", "Script to execute")]
        public string Script { get; set; }

        /// <summary>
        ///     Gets or sets the script file.
        /// </summary>
        [EventPropertyContract("ScriptFile", "Script from file")]
        public string ScriptFile { get; set; }

        /// <summary>
        ///     Gets or sets the message properties.
        /// </summary>
        [EventPropertyContract("MessageProperties", "MessageProperties")]
        public Dictionary<string, object> MessageProperties { get; set; }

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
        [EventActionContract("{3855F421-9451-45BD-8379-980F93FBB587}", "Main action", "Main action description")]
        public async Task Execute(ActionEvent actionEvent, ActionContext context)
        {
            try
            {
                Context = context;
                ActionEvent = actionEvent;
                var script = string.Empty;
                var metaProvider = new MetadataFileProvider();
                metaProvider.GetReference(context.GetType().Assembly.Location);
                var scriptEngine = new ScriptEngine(metaProvider);

                var session = scriptEngine.CreateSession(context);

                // TODO 1040
                session.AddReference(
                    @"C:\Users\ninoc\Documents\Visual Studio 2015\Projects\HybridIntegrationServices\Framework\bin\Debug\Framework.exe");
                session.AddReference(
                    @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.Windows.Forms.dll");

                // TODO 1041
                if (ScriptFile != null || ScriptFile != string.Empty)
                {
                    // ReSharper disable once AssignNullToNotNullAttribute
                    script = File.ReadAllText(ScriptFile);
                    session.ExecuteFile(script);
                }
                else
                {
                    session.Execute(Script);
                }

                return;
            }
            catch
            {
                // ignored
                return;
            }
        }
    }
}