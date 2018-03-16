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
using System.Runtime.Serialization;

#endregion

namespace SnapGate.Framework.ETW
{
    /// <summary>
    ///     The property bag.
    /// </summary>
    [Serializable]
    public sealed class PropertyBag : Dictionary<string, object>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyBag" /> class.
        /// </summary>
        public PropertyBag()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyBag" /> class.
        /// </summary>
        /// <param name="capacity">
        ///     The capacity.
        /// </param>
        public PropertyBag(int capacity)
            : base(capacity, StringComparer.Ordinal)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyBag" /> class.
        /// </summary>
        /// <param name="info">
        ///     The info.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        private PropertyBag(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}