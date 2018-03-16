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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Bubbling;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.Storage
{
    /// <summary>
    ///     Main persistent provider.
    /// </summary>
    public static class PersistentProvider
    {
        public enum CommunicationDiretion
        {
            OffRamp,

            OnRamp
        }

        public static bool PersistMessage(ActionContext actionContext)
        {
            if (ConfigurationBag.Configuration.EnablePersistingMessaging == false)
            {
                return true;
            }

            return PersistMessage(actionContext, CommunicationDiretion.OnRamp);
        }

        /// <summary>
        ///     Persist the message in local file system
        /// </summary>
        /// <param name="bubblingEvent">
        /// </param>
        /// <param name="actionContext"></param>
        /// <param name="communicationDiretion">
        ///     The communication Diretion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool PersistMessage(ActionContext actionContext, CommunicationDiretion communicationDiretion)
        {
            try
            {
                if (!ConfigurationBag.Configuration.EnablePersistingMessaging)
                {
                    return true;
                }

                var serializedMessage = JsonConvert.SerializeObject(actionContext.BubblingObjectBag);
                var directoryDate = string.Concat(
                    DateTime.Now.Year,
                    "\\",
                    DateTime.Now.Month.ToString().PadLeft(2, '0'),
                    "\\",
                    DateTime.Now.Day.ToString().PadLeft(2, '0'),
                    "\\",
                    communicationDiretion.ToString());
                var datetimeFile = string.Concat(
                    DateTime.Now.Year,
                    DateTime.Now.Month.ToString().PadLeft(2, '0'),
                    DateTime.Now.Day.ToString().PadLeft(2, '0'),
                    "-",
                    DateTime.Now.Hour.ToString().PadLeft(2, '0'),
                    "-",
                    DateTime.Now.Minute.ToString().PadLeft(2, '0'),
                    "-",
                    DateTime.Now.Second.ToString().PadLeft(2, '0'));

                var persistingForlder = Path.Combine(ConfigurationBag.Configuration.LocalStorageConnectionString,
                    directoryDate);
                Directory.CreateDirectory(persistingForlder);
                var filePersisted = Path.Combine(
                    persistingForlder,
                    string.Concat(datetimeFile, "-", actionContext.MessageId,
                        ConfigurationBag.MessageFileStorageExtension));

                File.WriteAllText(filePersisted, serializedMessage);
                Trace.WriteLine(
                    "Event persisted -  Consistency Transaction Point created.");
                return true;
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
                return false;
            }
        }

        /// <summary>
        ///     Persist the message in local file system
        /// </summary>
        /// <param name="bubblingObject">
        /// </param>
        /// <param name="eventActionContext"></param>
        /// <param name="communicationDiretion">
        ///     The communication Diretion.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public static bool PersistMessage(BubblingObject bubblingObject, string MessageId,
            CommunicationDiretion communicationDiretion)
        {
            try
            {
                if (!ConfigurationBag.Configuration.EnablePersistingMessaging)
                {
                    return true;
                }

                var serializedMessage = JsonConvert.SerializeObject(bubblingObject);
                var directoryDate = string.Concat(
                    DateTime.Now.Year,
                    "\\",
                    DateTime.Now.Month.ToString().PadLeft(2, '0'),
                    "\\",
                    DateTime.Now.Day.ToString().PadLeft(2, '0'),
                    "\\",
                    communicationDiretion.ToString());
                var datetimeFile = string.Concat(
                    DateTime.Now.Year,
                    DateTime.Now.Month.ToString().PadLeft(2, '0'),
                    DateTime.Now.Day.ToString().PadLeft(2, '0'),
                    "-",
                    DateTime.Now.Hour.ToString().PadLeft(2, '0'),
                    "-",
                    DateTime.Now.Minute.ToString().PadLeft(2, '0'),
                    "-",
                    DateTime.Now.Second.ToString().PadLeft(2, '0'));

                var persistingForlder = Path.Combine(ConfigurationBag.Configuration.LocalStorageConnectionString,
                    directoryDate);
                Directory.CreateDirectory(persistingForlder);
                var filePersisted = Path.Combine(
                    persistingForlder,
                    string.Concat(datetimeFile, "-", MessageId, ConfigurationBag.MessageFileStorageExtension));

                File.WriteAllText(filePersisted, serializedMessage);
                Trace.WriteLine("Event persisted -  Consistency Transaction Point created.");
                return true;
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    $"Error in {MethodBase.GetCurrentMethod().Name}",
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
                return false;
            }
        }
    }
}