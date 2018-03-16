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
using System.Linq;
using System.Reflection;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using SnapGate.Framework.Base;
using SnapGate.Framework.Deployment;
using SnapGate.Framework.Log;

#endregion

namespace SnapGate.Framework.Engine
{
    public class RestEventsEngine : IRestEventsEngine
    {
        //http://localhost:8000/SnapGate/Deploy?Configuration=Release&Platform=AnyCpu
        public string Deploy(string configuration, string platform)
        {
            try
            {
                string publishingFolder = Path.Combine(ConfigurationBag.DirectoryDeployment(),
                    ConfigurationBag.DirectoryNamePublishing);

                var regTriggers = new Regex(ConfigurationBag.DeployExtensionLookFor);
                var deployFiles =
                    Directory.GetFiles(publishingFolder, "*.*", SearchOption.AllDirectories)
                        .Where(
                            path =>
                                Path.GetExtension(path) == ".trigger" || Path.GetExtension(path) == ".event" ||
                                Path.GetExtension(path) == ".component");

                StringBuilder results = new StringBuilder();
                foreach (var file in deployFiles)
                {
                    string projectName = Path.GetFileNameWithoutExtension(publishingFolder + file);
                    string projectType = Path.GetExtension(publishingFolder + file).Replace(".", "");
                    bool resultOk = Jit.CompilePublishing(projectType, projectName,
                        configuration, platform);
                    if (resultOk)
                    {
                        string message = resultOk ? "without errors" : "with errors";
                        results.AppendLine(
                            $"{projectName}.{projectType} builded {message} check the {projectName}Build.Log file for more information");
                    }
                }

                return results.ToString();
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
                return ex.Message;
            }
        }

        //http://localhost:8000/SnapGate/SyncPush?ChannelID=*&PointID=*
        public string SyncPush(string channelId, string pointId)
        {
            SyncProvider.SendSyncPush(channelId, pointId);
            return $"Syncronization bag sent to Channel ID {channelId} and Point ID {pointId} - {DateTime.Now}";
        }

        //http://localhost:8000/SnapGate/SyncPull?ChannelID=*&PointID=*
        public string SyncPull(string channelId, string pointId)
        {
            SyncProvider.SendSyncPull(channelId, pointId);
            return
                $"Syncronization pull request sent to Channel ID {channelId} and Point ID {pointId} - {DateTime.Now}";
        }

        //http://localhost:8000/SnapGate/Sync
        public string Sync()
        {
            if (EventsEngine.SyncronizePoint())
                return $"Syncronization completed. - {DateTime.Now}";
            return "Point syncronization failed with errors, check the event viewer and the log.";
        }

        /// <summary>
        ///     Refresh the internal bubbling setting
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        /// http://localhost:8000/SnapGate/RefreshBubblingSetting
        public string RefreshBubblingSetting()
        {
            try
            {
                SyncProvider.RefreshBubblingSetting(true);
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
                return ex.Message;
            }

            return $"Syncronization Executed at {DateTime.Now}.";
        }


        /// <summary>
        ///     Execute an internal trigger
        /// </summary>
        /// <param name="triggerId">
        /// </param>
        /// <param name="configurationId"></param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        /// http://localhost:8000/SnapGate/ExecuteTrigger?ConfigurationID={5D793BC4-B111-4BF4-BAAF-196F661E13E2}&TriggerID={9A989BD1-C8DE-4FC1-B4BA-02E7D8A4AD76}&value=text
        public string ExecuteTrigger(string configurationId, string triggerId, string value)
        {
            try
            {
                var BubblingObjects = (from trigger in EventsEngine.BubblingTriggerConfigurationsSingleInstance
                    where trigger.IdComponent == triggerId && trigger.IdConfiguration == (configurationId ?? "")
                    select trigger).ToArray();
                if (BubblingObjects.Length != 0)
                {
                    byte[] content = EncodingDecoding.EncodingString2Bytes(value ?? "");
                    EventsEngine.ExecuteTriggerConfiguration(BubblingObjects[0], content);
                    return "Trigger executed.";
                }

                return
                    $"Trigger not found. - Looking for Trigger Id {triggerId} and configuration Id {configurationId}.";
            }
            catch (Exception ex)
            {
                return
                    $"Trigger not executed check the Windows event viewer and check the trigger Id and configuration Id are present and active in the trigger folder - Looking for Trigger Id {triggerId} and configuration Id {configurationId} - Exception {ex.Message}.";
            }
        }

        /// <summary>
        ///     Return the complete configuration
        /// </summary>
        /// <returns>
        ///     The <see cref="Stream" />.
        /// </returns>
        /// http://localhost:8000/SnapGate/Configuration
        public Stream Configuration()
        {
            try
            {
                return SyncProvider.GetConfiguration();
            }
            catch (Exception ex)
            {
                var docMain = new XmlDocument();
                var errorTemplate = docMain.CreateElement(string.Empty, "Error", string.Empty);
                var errorText = docMain.CreateTextNode(ex.Message);
                errorTemplate.AppendChild(errorText);

                var currentWebContext = WebOperationContext.Current;
                if (currentWebContext != null)
                {
                    currentWebContext.OutgoingResponse.ContentType = "text/xml";
                }

                return new MemoryStream(EncodingDecoding.EncodingString2Bytes(errorTemplate.OuterXml));
            }
        }
    }
}