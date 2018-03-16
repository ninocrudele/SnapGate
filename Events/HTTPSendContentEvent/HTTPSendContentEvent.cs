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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Log;

#endregion

namespace HTTPSendContentEvent.Event
{
    /// <summary>
    ///     The no operation event.
    /// </summary>
    [EventContract("{8c87cf14-7a9c-4a62-91b5-d47cd57695d8}", "HTTPSendContentEvent",
        "HTTPSendContentEvent Event component", true)]
    public class HTTPSendContentEvent : IEventType
    {
        [EventPropertyContract("uri", "Destination service endpoint")]
        public string uri { get; set; }

        [EventPropertyContract("ContentType", "Message ContentType")]
        public string ContentType { get; set; }

        [EventPropertyContract("WebrequestMethod", "WebrequestMethod GET or POST")]
        public string WebrequestMethod { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action event.
        /// </summary>
        public ActionEvent ActionEvent { get; set; }


        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        [EventPropertyContract("DataContext", "Main data context")]
        public byte[] DataContext { get; set; }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="actionEvent">
        ///     The set event action event.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        [EventActionContract("{83029d5b-dd61-4184-a884-f3b937ce2da1}", "Main action",
            "Main action executed by the event")]
        public async Task Execute(ActionEvent actionEvent, ActionContext context)
        {
            //ExecuteMethod(actionEvent, context);
            await Task.Run(() =>
            {
                ExecuteMethod(actionEvent, context);
                string a = "";
            });


            return;
        }

        static async void WriteCharacters()
        {
            using (StreamWriter writer = File.CreateText($"C:\\test\\test\\{Guid.NewGuid().ToString()}"))
            {
                await writer.WriteAsync("a");
            }
        }

        private async Task ExecuteMethod(ActionEvent actionEvent, ActionContext context)
        {
            try
            {
                // declare httpwebrequet wrt url defined above
                HttpWebRequest webrequest = (HttpWebRequest) WebRequest.Create(uri);
                // set method as post
                webrequest.Method = WebrequestMethod;
                // set content type
                webrequest.ContentType = ContentType;
                // set content length
                webrequest.ContentLength = DataContext.Length;
                // get stream data out of webrequest object
                Stream newStream = webrequest.GetRequestStream();
                newStream.Write(DataContext, 0, DataContext.Length);
                newStream.Close();
                // declare & read response from service
                HttpWebResponse webresponse = (HttpWebResponse) webrequest.GetResponse();
                using (var stream = webresponse.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    StreamReader streamReader = new StreamReader(stream);
                    string message = streamReader.ReadToEnd();
                    DataContext = Encoding.Default.GetBytes(message);
                    actionEvent(this, context);
                }
            }
            catch (Exception e)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name} in HTTPSendContentEvent. ",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    e,
                    Constant.LogLevelError);
            }

            //using (StreamWriter writer = File.CreateText($"C:\\test\\test\\{Guid.NewGuid().ToString()}"))
            //{
            //    writer.Write("a4");
            //}
            Thread.Sleep(1000);
        }
    }

    public class AsyncAwait
    {
        public async Task DoStuff()
        {
            await Task.Run(() => { LongRunningOperation(); });
        }

        private static async Task<string> LongRunningOperation()
        {
            int counter;

            for (counter = 0; counter < 50000; counter++)
            {
                Console.WriteLine(counter);
            }

            Thread.Sleep(1000);
            return "Counter = " + counter;
        }
    }
}