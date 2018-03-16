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
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;

#endregion

namespace SnapGate.Framework.Contracts
{
    public class TriggerEmbeddedBag
    {
        public ActionTrigger DelegateActionTrigger { get; set; }
        public ActionContext ActionContextTrigger { get; set; }

        public BaseAction BaseActionTrigger { get; set; }
        public object[] Parameters { get; set; }
        public ITriggerType ITriggerTypeInstance { get; set; }
        public IEventType IEventTypeInstance { get; set; }
        public ActionContext ActionContext { get; set; }

        public List<Property> Properties { get; set; }
    }
}