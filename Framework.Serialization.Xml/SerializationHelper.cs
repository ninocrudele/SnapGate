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
using Newtonsoft.Json;
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Channels;
using SnapGate.Framework.Contracts.Configuration;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Points;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace SnapGate.Framework.Serialization.Xml
{
    /// <summary>
    ///     TODO The serialization helper.
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        ///     TODO The crete json trigger configuration template.
        /// </summary>
        /// <param name="triggerObject">
        ///     TODO The bubbling event.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string CreteJsonTriggerConfigurationTemplate(ITriggerAssembly triggerObject)
        {
            var eventCorrelationTemplate = new Event(
                "{Event component ID to execute if Correlation = true}",
                "{Configuration ID to execute if Correlation = true}",
                "Event Name Sample",
                "Event Description Sample");
            try
            {
                var triggerConfiguration = new TriggerConfiguration();
                triggerConfiguration.Trigger = new Trigger(
                    triggerObject.Id,
                    Guid.NewGuid().ToString(),
                    triggerObject.Name,
                    triggerObject.Description);
                triggerConfiguration.Trigger.TriggerProperties = new List<TriggerProperty>();
                foreach (var Property in triggerObject.Properties)
                {
                    if (Property.Value.Name != "DataContext")
                    {
                        var triggerProperty = new TriggerProperty(Property.Value.Name, "Value to set");
                        triggerConfiguration.Trigger.TriggerProperties.Add(triggerProperty);
                    }
                }

                triggerConfiguration.Events = new List<Event>();

                // 1*
                var eventTriggerTemplate = new Event(
                    "{Event component ID  Sample to execute}",
                    "{Configuration ID  Sample to execute}",
                    "Event Name Sample",
                    "Event Description Sample");
                eventTriggerTemplate.Channels = new List<Channel>();
                var points = new List<Point>();
                points.Add(new Point("Point ID Sample", "Point Name Sample", "Point Description Sample"));
                eventTriggerTemplate.Channels.Add(
                    new Channel("Channel ID Sample", "Channel Name Sample", "Channel Description Sample", points));

                eventCorrelationTemplate.Channels = new List<Channel>();
                eventCorrelationTemplate.Channels.Add(
                    new Channel("Channel ID Sample", "Channel Name Sample", "Channel Description Sample", points));

                triggerConfiguration.Events.Add(eventTriggerTemplate);

                var events = new List<Event>();
                events.Add(eventCorrelationTemplate);
                eventTriggerTemplate.Correlation = new Correlation("Correlation Name Sample", "C# script", events);

                var serializedMessage = JsonConvert.SerializeObject(
                    triggerConfiguration,
                    Formatting.Indented,
                    new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});

                // string serializedMessage = JsonConvert.SerializeObject(triggerConfiguration);
                return serializedMessage;

                // return "<![CDATA[" + serializedMessage + "]]>";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        ///     TODO The crete json event configuration template.
        /// </summary>
        /// <param name="eventObject">
        ///     TODO The bubbling event.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string CreteJsonEventConfigurationTemplate(IEventAssembly eventObject)
        {
            try
            {
                var eventConfiguration = new EventConfiguration();
                eventConfiguration.Event = new Event(
                    eventObject.Id,
                    "{Configuration ID to execute}",
                    eventObject.Name,
                    eventObject.Description);

                eventConfiguration.Event.EventProperties = new List<EventProperty>();
                foreach (var Property in eventObject.Properties)
                {
                    if (Property.Value.Name != "DataContext")
                    {
                        var eventProperty = new EventProperty(Property.Value.Name, "Value to set");
                        eventConfiguration.Event.EventProperties.Add(eventProperty);
                    }
                }

                var eventCorrelationTemplate = new Event(
                    "{Event component ID to execute if Correlation = true}",
                    "{Configuration ID to execute if Correlation = true}",
                    "EventName",
                    "EventDescription");
                eventCorrelationTemplate.Channels = new List<Channel>();
                var points = new List<Point>();
                points.Add(new Point("Point ID", "Point Name", "Point Description"));
                eventCorrelationTemplate.Channels.Add(
                    new Channel("Channel ID", "Channel Name", "Channel Description", points));

                var events = new List<Event>();
                events.Add(eventCorrelationTemplate);
                eventConfiguration.Event.Channels = new List<Channel>();
                eventConfiguration.Event.Channels.Add(
                    new Channel("Channel ID", "Channel Name", "Channel Description", points));

                eventConfiguration.Event.Correlation = new Correlation("Correlation Name", "C# script", events);

                var serializedMessage = JsonConvert.SerializeObject(
                    eventConfiguration,
                    Formatting.Indented,
                    new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
                return serializedMessage;

                // return "<![CDATA[" + serializedMessage + "]]>";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}