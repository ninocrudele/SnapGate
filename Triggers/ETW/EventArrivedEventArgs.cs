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

#endregion

namespace SnapGate.Framework.ETW
{
    public sealed class EventArrivedEventArgs : EventArgs
    {
        // Keep this event small.
        /// <summary>
        ///     Initializes a new instance of the <see cref="EventArrivedEventArgs" /> class.
        /// </summary>
        /// <param name="error">
        ///     The error.
        /// </param>
        internal EventArrivedEventArgs(Exception error)
            : this(0 /*eventId*/, new PropertyBag())
        {
            Error = error;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EventArrivedEventArgs" /> class.
        /// </summary>
        /// <param name="eventId">
        ///     The event id.
        /// </param>
        /// <param name="properties">
        ///     The properties.
        /// </param>
        internal EventArrivedEventArgs(ushort eventId, PropertyBag properties)
        {
            EventId = eventId;
            Properties = properties;
        }

        /// <summary>
        ///     Gets the event id.
        /// </summary>
        public ushort EventId { get; }

        /// <summary>
        ///     Gets the properties.
        /// </summary>
        public PropertyBag Properties { get; }

        /// <summary>
        ///     Gets the error.
        /// </summary>
        public Exception Error { get; }
    }
}