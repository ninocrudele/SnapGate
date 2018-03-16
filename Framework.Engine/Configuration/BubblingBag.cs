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
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Configuration;

#endregion

namespace SnapGate.Framework.Engine
{
    /// <summary>
    ///     Contains the bubbling folder filse (trg, evn, and dlls)
    /// </summary>
    [Serializable, DataContract]
    public class BubblingBagObjet
    {
        [DataMember] public List<TriggerConfiguration> TriggerConfigurationList { get; set; }

        [DataMember] public Dictionary<string, EventConfiguration> EventConfigurationList { get; set; }

        [DataMember] public List<ChainConfiguration> ChainConfigurationList { get; set; }

        [DataMember] public List<ComponentConfiguration> ComponentConfigurationList { get; set; }

        [DataMember] public List<BubblingObject> GlobalEventListBaseDll { get; set; }

        [DataMember] public Configuration Configuration { get; set; }
    }

    [Serializable]
    public class BubblingBag
    {
        public byte[] contentBubblingFolder { get; set; }
    }
}