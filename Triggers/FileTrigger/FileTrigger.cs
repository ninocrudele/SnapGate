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
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Globals;
using SnapGate.Framework.Contracts.Triggers;
using SnapGate.Framework.Log;
using SnapGate.Framework.PBI;

#endregion

namespace SnapGate.Framework.FileTrigger
{
    /// <summary>
    ///     The file trigger.
    /// </summary>
    [TriggerContract("{3C62B951-C353-4899-8670-C6687B6EAEFC}", "FileTrigger",
        "Get the content from file in a specific directory or shared forlder.", false, true, false)]
    public class FileTrigger : ITriggerType
    {
        /// <summary>
        ///     Gets or sets the regex file pattern.
        /// </summary>
        [TriggerPropertyContract("RegexFilePattern", "File pattern, could be a reular expression")]
        public string RegexFilePattern { get; set; }

        [TriggerPropertyContract("RegexSearchPattern", "File pattern, could be a reular expression")]
        public string RegexSearchPattern { get; set; }

        [TriggerPropertyContract("MultiValues", "File pattern, could be a reular expression")]
        public bool MultiValues { get; set; }

        [TriggerPropertyContract("KeepFirstLine", "File pattern, could be a reular expression")]
        public bool KeepFirstLine { get; set; }

        /// <summary>
        ///     Gets or sets the polling time.
        /// </summary>
        [TriggerPropertyContract("BatchFilesSize", "Number of file to receive fro each batch.")]
        public int BatchFilesSize { get; set; }

        /// <summary>
        ///     Gets or sets the polling time.
        /// </summary>
        [TriggerPropertyContract("PollingTime", "Polling time.")]
        public int PollingTime { get; set; }

        /// <summary>
        ///     Gets or sets the done extension name.
        /// </summary>
        [TriggerPropertyContract("DoneExtensionName", "Rename extension file received.")]
        public string DoneExtensionName { get; set; }

        /// <summary>
        ///     Gets or sets the input directory.
        /// </summary>
        [TriggerPropertyContract("InputDirectory", "Input Directory location")]
        public string InputDirectory { get; set; }

        private PBIEngineProvider pBIEngineProvider { get; set; }

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action trigger.
        /// </summary>
        public ActionTrigger ActionTrigger { get; set; }

        /// <summary>
        ///     If must be syncronous
        /// </summary>
        public bool Syncronous { get; set; }

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        [TriggerPropertyContract("DataContext", "Trigger Default Main Data")]
        public byte[] DataContext { get; set; }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="actionTrigger">
        ///     The set event action trigger.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        [TriggerActionContract("{58EEAFEF-CF6A-44C3-9BB9-81EFD680CA36}", "Main action", "Main action description")]
        public byte[] Execute(ActionTrigger actionTrigger, ActionContext context)
        {
            try
            {
                bool PowerBIEngineInstatiated = false;
                //context.EventBehaviour = EventBehaviour.SyncAsync;
                //context.SyncAsyncEventAction = SyncAsyncActionReceived;
                while (true)
                {
                    var files =
                        Directory.GetFiles(InputDirectory, "*.*", SearchOption.AllDirectories)
                            .Where(path => Path.GetExtension(path) == RegexFilePattern)
                            .ToArray();

                    if (files.Length != 0)
                    {
                        dynamic expandoObject = new ExpandoObject();
                        var dataDomain = new List<dynamic>();

                        if (ConfigurationBag.Configuration.PowerBIEnabled && !PowerBIEngineInstatiated)
                        {
                            //Load configuration

                            LogEngine.DirectEventViewerLog("Start PowerBI BAM for invoices", 4);


                            expandoObject.Quantity = 0;
                            expandoObject.ProductCode = "";
                            expandoObject.GrossAmount = 0;
                            expandoObject.NetAmount = 0;
                            expandoObject.Quantity = 0;
                            expandoObject.Discount = 0;
                            expandoObject.Location = "";
                            expandoObject.Date = DateTime.Now.ToShortDateString();
                            dataDomain.Add(expandoObject);
                            pBIEngineProvider = new PBIEngineProvider();
                            pBIEngineProvider.CreatePBIProvider(dataDomain, "TotalIncomingLocation", "TotalIncoming");
                            PowerBIEngineInstatiated = true;
                        }

                        Random rnd = new Random();

                        expandoObject.ProductCode = "X034" + rnd.Next(10, 50);
                        expandoObject.Quantity = rnd.Next(1, 50);
                        expandoObject.GrossAmount = rnd.Next(1000, 5000);
                        expandoObject.Discount = rnd.Next(1, 50);
                        expandoObject.NetAmount = ((expandoObject.GrossAmount - 400) * expandoObject.Discount) / 100;
                        expandoObject.Location = ConfigurationBag.Configuration.PointName;

                        string Date = DateTime.Parse($"{rnd.Next(1, 12)}/{rnd.Next(1, 12)}/2017").ToShortDateString();
                        expandoObject.Date = Date;
                        dataDomain.Add(expandoObject);
                        pBIEngineProvider.SendData(dataDomain);

                        int numberOfBatch = 0;
                        if (files.Length >= BatchFilesSize)
                            numberOfBatch = BatchFilesSize;
                        else
                            numberOfBatch = files.Length;
                        string file = string.Empty;

                        for (int i = 0; i < numberOfBatch; i++)
                        {
                            file = files[i];
                            var data = File.ReadAllBytes(file);
                            if (RegexSearchPattern.Length > 0)
                            {
                                string value = SearchValues(MultiValues, KeepFirstLine, RegexSearchPattern, file);
                                if (value.Length > 0 && !MultiValues)
                                {
                                    File.Delete(Path.ChangeExtension(file, DoneExtensionName));
                                    File.Move(file, Path.ChangeExtension(file, DoneExtensionName));
                                    DataContext = data;
                                    actionTrigger(this, context);
                                }

                                if (value.Length > 0 && MultiValues)
                                {
                                    File.Delete(Path.ChangeExtension(file, DoneExtensionName));
                                    File.Move(file, Path.ChangeExtension(file, DoneExtensionName));
                                    DataContext = Encoding.UTF8.GetBytes(value);
                                    actionTrigger(this, context);
                                }
                            }
                            else
                            {
                                File.Delete(Path.ChangeExtension(file, DoneExtensionName));
                                File.Move(file, Path.ChangeExtension(file, DoneExtensionName));
                                DataContext = data;
                                actionTrigger(this, context);
                            }
                        }
                    }

                    Thread.Sleep(PollingTime);
                }
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


                actionTrigger(this, null);
                return null;
            }
        }

        static string SearchValues(bool multiValues, bool keepFirstLine, string RegexSearchPattern, string outputFile)
        {
            try
            {
                Regex test = new Regex(RegexSearchPattern);
                using (StreamReader file = new StreamReader(outputFile))
                {
                    string line = String.Empty;
                    string firstLine = String.Empty;
                    StringBuilder newContent = new StringBuilder();
                    string valueReturn = String.Empty;

                    {
                        if (keepFirstLine)
                        {
                            firstLine = file.ReadLine();
                            newContent.AppendLine(firstLine);
                        }

                        while ((line = file.ReadLine()) != null)

                            if (test.Matches(line).Count > 0)
                            {
                                if (!multiValues)
                                    return "OK";
                                newContent.AppendLine(line);
                            }
                    }
                    if (!multiValues)
                        return "";
                    return newContent.ToString();
                }
            }
            catch
            {
                return "";
            }
        }

        //private void SyncAsyncActionReceived(byte[] DataContext)
    }
}