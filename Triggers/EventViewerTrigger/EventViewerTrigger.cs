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
using System.Runtime.Serialization;
using System.Threading;
using Newtonsoft.Json;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace SnapGate.Framework.EventViewerTrigger
{
    /// <summary>
    ///     The event viewer trigger.
    /// </summary>
    [TriggerContract("{0E8D9421-E749-4B0D-ADCE-03D4A6568998}", "Event Viewer Trigger", "Intercept Event Viewer Message",
        false, true, false)]
    public class EventViewerTrigger : ITriggerType
    {
        /// <summary>
        ///     Gets or sets the event log.
        /// </summary>
        [TriggerPropertyContract("EventLog", "Event Source to monitor")]
        public string EventLog { get; set; }

        /// <summary>
        /// Pre collect the information
        /// </summary>
        [TriggerPropertyContract("PreCollectData", "Precollect data before starting")]
        public bool PreCollectData { get; set; }

        /// <summary>
        /// Source Location
        /// </summary>
        [TriggerPropertyContract("Location", "Event source location")]
        public string Location { get; set; }

        /// <summary>
        /// Source Location
        /// </summary>
        [TriggerPropertyContract("Days", "Number of days to retreive")]
        public int Days { get; set; }

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
        [TriggerActionContract("{25F85716-1154-4473-AFFE-F8F4E8AC17A9}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                Context = context;
                ActionTrigger = actionTrigger;

                var myNewLog = new EventLog(EventLog);

                if (PreCollectData)
                {
                    EventLogEntryCollection eventLogEntryCollection = myNewLog.Entries;

                    foreach (EventLogEntry eventLogEntry in eventLogEntryCollection)
                    {
                        var eventViewerMessage = new EventViewerMessage
                        {
                            Location = Location,
                            EntryType = eventLogEntry.EntryType,
                            MachineName = eventLogEntry.MachineName,
                            Message = eventLogEntry.Message,
                            Source = eventLogEntry.Source,
                            TimeWritten = eventLogEntry.TimeWritten
                        };
                        var serializedMessage = JsonConvert.SerializeObject(eventViewerMessage);
                        DataContext = EncodingDecoding.EncodingString2Bytes(serializedMessage);
                        ActionTrigger(this, Context);
                    }
                }


                myNewLog.EntryWritten += MyOnEntryWritten;
                myNewLog.EnableRaisingEvents = true;

                Thread.Sleep(Timeout.Infinite);
                return null;
            }
            catch (Exception ex)
            {
                // ignored
                actionTrigger(this, null);
                return null;
            }
        }

        /// <summary>
        ///     The my on entry written.
        /// </summary>
        /// <param name="source">
        ///     The source.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        public void MyOnEntryWritten(object source, EntryWrittenEventArgs e)
        {
            var eventViewerMessage = new EventViewerMessage
            {
                EntryType = e.Entry.EntryType,
                MachineName = e.Entry.MachineName,
                Message = e.Entry.Message,
                Source = e.Entry.Source,
                TimeWritten = e.Entry.TimeWritten
            };
            var serializedMessage = JsonConvert.SerializeObject(eventViewerMessage);

            DataContext = EncodingDecoding.EncodingString2Bytes(serializedMessage);
            ActionTrigger(this, Context);
        }
    }

    /// <summary>
    ///     The event viewer message.
    /// </summary>
    [DataContract]
    internal class EventViewerMessage
    {
        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        [DataMember]
        public string Location { get; set; }

        /// <summary>
        ///     Gets or sets the source.
        /// </summary>
        [DataMember]
        public string Source { get; set; }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        ///     Gets or sets the machine name.
        /// </summary>
        [DataMember]
        public string MachineName { get; set; }

        /// <summary>
        ///     Gets or sets the entry type.
        /// </summary>
        [DataMember]
        public EventLogEntryType EntryType { get; set; }

        /// <summary>
        ///     Gets or sets the time written.
        /// </summary>
        [DataMember]
        public DateTime TimeWritten { get; set; }
    }
}