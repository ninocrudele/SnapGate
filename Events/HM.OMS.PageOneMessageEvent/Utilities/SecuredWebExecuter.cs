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
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

#endregion

namespace HM.OMS.PageOneMessageConsole.Utilities
{
    public class SecuredWebExecuter
    {
        public string Execute(string url)
        {
            try
            {
                // use the SSL protocol (instead of TLS)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

                // ignore any certificate complaints
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => { return true; };

                // create HTTP web request with proper content type
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.ContentType = "application/xml;charset=UTF8";

                // grab the PFX as a X.509 certificate from disk
                string certFileName = Path.Combine("webservice.pfx");

                // load the X.509 certificate and add to the web request
                X509Certificate cert = new X509Certificate(certFileName, "(top-secret password)");
                request.ClientCertificates.Add(cert);
                request.PreAuthenticate = true;

                // call the web service and get response
                WebResponse response = request.GetResponse();

                Stream responseStream = response.GetResponseStream();
            }
            catch (Exception)
            {
                // log and print out error
            }

            return null;
        }
    }
}