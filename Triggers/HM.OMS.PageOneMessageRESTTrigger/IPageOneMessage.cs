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

using System.ServiceModel;
using System.ServiceModel.Web;

#endregion

namespace HM.OMS.PageOneMessageRESTTrigger
{
    [ServiceContract]
    public interface IPageOneMessage
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "json")]
        string SendMessage(iPageOneMessage pageOneMessage);

        [OperationContract]
        [WebGet]
        bool ServiceAvailable();

        [OperationContract]
        [WebGet]
        string Auth();
    }

    public interface iPageOneMessage
    {
        string From { get; set; }
        string To { get; set; }
        string Message { get; set; }
    }
}