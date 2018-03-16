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
using System.Reflection;
using SnapGate.Framework.Contracts.Bubbling;

#endregion

namespace SnapGate.Framework.Contracts.Events
{
    public class EventAssembly : IEventAssembly
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Shared { get; set; }
        public string PollingRequired { get; set; }
        public string Nop { get; set; }
        public Version Version { get; set; }
        public byte[] AssemblyContent { get; set; }
        public IEventType EventType { get; set; }
        public Assembly AssemblyObject { get; set; }
        public Type AssemblyClassType { get; set; }
        public string AssemblyFile { get; set; }
        public List<BaseAction> BaseActions { get; set; }
        public Dictionary<string, Property> Properties { get; set; }
    }
}