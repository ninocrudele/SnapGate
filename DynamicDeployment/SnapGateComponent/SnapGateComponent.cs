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

using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Components;

#endregion

namespace SnapGate.Framework.BTSTransformComponent
{
    //<USING>

    [ComponentContract("{*ID*}", "*NAME*", "*DESCRIPTION*")]
    public class SnapGateComponent : IChainComponentType
    {
        //<PROPERTIES>

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        /// <value>
        ///     The data context.
        /// </value>
        [ComponentPropertyContract("DataContext", "Main data context")]
        public byte[] DataContext { get; set; }

        [ComponentActionContract("{*CONTRACTID*}", "Main action", "Main action executed by the component")]
        public byte[] Execute()
        {
            //<MAINCODE>
            return EncodingDecoding.EncodingString2Bytes("result");
        }

        //<FUNCTIONS>
    }
}