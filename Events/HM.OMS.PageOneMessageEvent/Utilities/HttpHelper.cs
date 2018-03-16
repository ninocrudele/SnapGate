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
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using NLog;

#endregion

namespace HMVN.OMS.Common.Utils
{
    public static class HttpHelper
    {
        private static readonly Logger log = LogManager.GetLogger("HttpHelper");

        public static byte[] StreamToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        public static byte[] ReadAllBytes(string fileName)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int) fs.Length);
            }

            return buffer;
        }

        public static string DictToString(Dictionary<string, object> dict)
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, object> kvp in dict)
                builder.Append(kvp.Key + "=" + kvp.Value + "&");

            if (builder.Length > 0) builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        public static string DictToString(NameValueCollection collection)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string key in collection.AllKeys)
                builder.Append(key + "=" + collection[key] + "&");

            if (builder.Length > 0) builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        public static string UrlParamCombine(string url, string paramsString)
        {
            if (url.Contains("?"))
                url = string.Concat(url, "&", paramsString);
            else
                url = string.Concat(url, "?", paramsString);
            return url;
        }

        public static string UrlParamCombine(string url, Dictionary<string, object> urlparams)
        {
            return UrlParamCombine(url, DictToString(urlparams));
        }

        public static string UrlPathCombine(string left, params object[] right)
        {
            if (right.Length == 0 || string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right[0].ToString()))
                return left;

            string curRight = right[0].ToString();

            // Add "/" to last char of "left"
            if (left.LastIndexOf("/") != left.Length - 1)
                left += "/";

            // Remove "/" of first char of right
            if (curRight.IndexOf("/") == 0)
                curRight = curRight.Substring(1);

            return UrlPathCombine(string.Concat(left, curRight), right.Skip(1).ToArray());
        }

        public static string CatchWebRequestException(WebException wexc)
        {
            string msg = wexc.Message ?? "Response is null!.";
            if (wexc != null && wexc.Response != null)
                using (WebResponse response = wexc.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse) response;
                    log.Error("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        msg = reader.ReadToEnd();
                    }
                }
            else if (wexc.InnerException != null)
                msg = wexc.InnerException.Message;

            return msg;
        }

        public static T ParseJsonResult<T>(string result)
        {
            try
            {
                if (result.Length > 0)
                {
                    var type = typeof(T);
                    if (type == typeof(String))
                    {
                        if (result.StartsWith("\""))
                            result = result.Substring(1, result.Length - 2);
                        return (T) Convert.ChangeType(result, type);
                    }
                }

                T data = JsonConvert.DeserializeObject<T>(result);
                return data;
            }
            catch (Exception ex)
            {
                string mes = result + " ----------- " + ex.Message + " -------------- " + ex.StackTrace;
                WriteLogError(result);
                WriteLogError(mes);
                return default(T);
            }
        }

        #region Logs

        public static void WriteLogError(Exception ex)
        {
            string message = CursiveExceptionMessage(ex);
            WriteLogError(message);
        }

        public static void WriteLogError(string message)
        {
            log.Error(message);
        }

        public static string CursiveExceptionMessage(Exception ex)
        {
            string message = ex.Message + " --------------------- " + ex.StackTrace;
            while (ex.InnerException != null)
            {
                message += "\r\n\n" + ex.InnerException.Message + " --------------------- " +
                           ex.InnerException.StackTrace;
                ex = ex.InnerException;
            }

            return message;
        }

        public static void WriteLogMessage(Exception ex)
        {
            string message = ex.Message + " --------------------- " + ex.StackTrace;
            while (ex.InnerException != null)
            {
                message += "\r\n\n" + ex.InnerException.Message + " --------------------- " +
                           ex.InnerException.StackTrace;
                ex = ex.InnerException;
            }

            WriteLogMessage(message);
        }

        public static void WriteLogMessage(string message)
        {
            log.Info(message);
        }

        #endregion
    }
}