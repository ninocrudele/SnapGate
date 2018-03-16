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
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Threading;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Globals;

#endregion

namespace SnapGate.Framework.DynamicRESTTrigger
{
    public class DynamicRestService : IDynamicRest
    {
        private static Func<ActionTrigger, ActionContext> _getDataTrigger;

        static WebServiceHost engineHost;

        public string GetValue(string fromValue)
        {
            //execute the trigger event
            //witfor
            // RunTheMethod GetDataTrigger();
            return "cazzo";
        }

        public static bool StartService(string webApiEndPoint, Func<ActionTrigger, ActionContext> getDataTrigger)
        {
            _getDataTrigger = getDataTrigger;

            engineHost = new WebServiceHost(typeof(DynamicRestService), new Uri(webApiEndPoint));
            engineHost.AddServiceEndpoint(typeof(DynamicRestService), new WebHttpBinding(),
                ConfigurationBag.EngineName);
            var stp = engineHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            stp.HttpHelpPageEnabled = false;
            engineHost.Open();
            Thread.Sleep(Timeout.Infinite);
            return true;
        }
    }
}