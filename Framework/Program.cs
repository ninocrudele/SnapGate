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
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Windows.Forms;
using SnapGate.Framework.Base;
using SnapGate.Framework.Engine;
using SnapGate.Framework.Log;
using SnapGate.Framework.NTService;

#endregion

namespace SnapGate.Framework
{
    /// <summary>
    ///     Class containing the main entry to the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Handles the ProcessExit event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        public static void CurrentDomainProcessExit(object sender, EventArgs e)
        {
            CoreEngine.StopEventEngine();
        } // CurrentDomain_ProcessExit

        /// <summary>
        ///     Mains the main entry to the program.
        /// </summary>
        /// <param name="args">The arguments to the program.</param>
        /// <exception cref="System.NotImplementedException">
        ///     Exception thrown if incorrect parameters are passed to the command-line.
        /// </exception>
        public static void Main(string[] args)
        {
            try
            {
                LogEngine.Init();
                Trace.WriteLine(
                    $"Version {Assembly.GetExecutingAssembly().GetName().Version}");

                if (!Environment.UserInteractive)
                {
                    Trace.WriteLine(
                        "SnapGate-services To Run - Not UserInteractive Environment the service will start in ServiceBase mode.");
                    if (args.Length > 0)
                    {
                        //Run in batch and console mode
                        Trace.WriteLine(
                            $"SnapGate-services To Run - Command line > 0 start NT Service mode . args = {args[0]}.");
                        switch (args[0].ToUpper())
                        {
                            case "S":
                                Trace.WriteLine("SnapGate-services To Run - Service Fabric mode requested.");
                                Trace.WriteLine(
                                    "--SnapGate Sevice Initialization--Start Engine.");
                                CoreEngine.StartEventEngine(null);
                                Console.WriteLine("\rEngine started...");
                                Console.ReadLine();
                                break;
                            case "M":
                                AppDomain.CurrentDomain.ProcessExit += CurrentDomainProcessExit;
                                Trace.WriteLine(
                                    "--SnapGate Sevice Initialization--Start Engine.");
                                CoreEngine.StartEventEngine(null);
                                Console.WriteLine("\rEngine started...");
                                Console.ReadLine();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        Trace.WriteLine("SnapGate-services To Run - Command line = 0 start NT Service mode.");
                        Trace.WriteLine(
                            $"SnapGate-services To Run - Environment.OSVersion:{Environment.OSVersion} Environment.Version:{Environment.Version}");
                        Trace.WriteLine("SnapGate-services To Run procedure initialization.");
                        ServiceBase[] servicesToRun = {new NTWindowsService()};
                        Trace.WriteLine("SnapGate-services To Run procedure starting.");
                        ServiceBase.Run(servicesToRun);
                    }
                }
                else
                {
                    if (args.Length == 0)
                    {
                        // Set Console windows
                        Console.Title = ConfigurationBag.Configuration.PointName;
                        Console.SetWindowPosition(0, 0);
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(@"[M] Run SnapGate in MS-DOS Console mode.");
                        Console.WriteLine(@"[I] Install SnapGate Windows NT Service.");
                        Console.WriteLine(@"[U] Uninstall SnapGate Windows NT Service.");
                        Console.WriteLine(@"[O] Clone a new SnapGate Point.");
                        Console.WriteLine(@"[Ctrl + C] Exit.");
                        Console.ForegroundColor = ConsoleColor.White;
                        var consoleKeyInfo = Console.ReadKey();

                        string param1 = ";";

                        switch (consoleKeyInfo.Key)
                        {
                            case ConsoleKey.M:
                                AppDomain.CurrentDomain.ProcessExit += CurrentDomainProcessExit;
                                Trace.WriteLine("SnapGate Sevice Initialization - Start Engine.");
                                CoreEngine.StartEventEngine(null);
                                Trace.WriteLine("SnapGate Engine Started.");
                                Console.WriteLine("\rEngine started...");
                                Console.ReadLine();
                                break;
                            case ConsoleKey.C:
                                //string CloneName = AskInputLine("Enter the Clone name.");
                                Process.Start(Path.Combine(Application.StartupPath, "Create new Clone.cmd"));
                                break;
                            case ConsoleKey.I:
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Clear();
                                Console.WriteLine("Specify the Windows NT Service Name and press Enter:");
                                CoreNtService.ServiceName = AskInputLine("Specify the Windows NT Service Name:");
                                CoreNtService.InstallService();
                                break;
                            case ConsoleKey.U:
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Clear();
                                Console.WriteLine("Specify the Windows NT Service Name and press Enter:");
                                CoreNtService.ServiceName = AskInputLine("Specify the Windows NT Service Name:");
                                CoreNtService.StopService();
                                CoreNtService.UninstallService();
                                break;
                            case ConsoleKey.S:
                                //string CloneName = AskInputLine("Enter the Clone name.");
                                Process.Start(Path.Combine(Application.StartupPath, "Create new Clone.cmd"));
                                break;
                            case ConsoleKey.B:
                                param1 = AskInputLine("Enter the BizTalk installation folder.");
                                Process.Start(Path.Combine(Application.StartupPath, $"Create new Clone.cmd {param1}"));
                                break;
                            case ConsoleKey.O:
                                //string CloneName = AskInputLine("Enter the Clone name.");
                                Process.Start(Path.Combine(Application.StartupPath, "Create new Clone.cmd"));
                                break;
                        }
                    } //if
                    else
                    {
                        //Run in batch and console mode
                        if (args.Length == 1 && args[0].ToUpper() == "M")
                        {
                            Trace.WriteLine("SnapGate Sevice Initialization--Start Engine.");
                            CoreEngine.StartEventEngine(null);
                            Trace.WriteLine("SnapGate Engine Started.");
                            Console.WriteLine("\rEngine started...");
                            Console.ReadLine();
                        }
                        else if (args.Length == 2 && args[0].ToUpper() == "I")
                        {
                            CoreNtService.ServiceName = string.Concat("SnapGate", args[1]);
                            CoreNtService.InstallService();
                            Environment.Exit(0);
                        }
                        else if (args.Length == 2 && args[0].ToUpper() == "U")
                        {
                            CoreNtService.ServiceName = string.Concat("SnapGate", args[1]);
                            CoreNtService.StopService();
                            CoreNtService.UninstallService();
                            Environment.Exit(0);
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine(
                                @"SnapGate [M]  [I] [U]",
                                ConsoleColor.Green);
                            Console.WriteLine("M", ConsoleColor.DarkGreen);
                            Console.WriteLine(
                                @"[M] Execute SnapGate in MS Dos console mode.",
                                ConsoleColor.Green);
                            Console.WriteLine(
                                "[I servicename] Install SnapGate as Windows NT Service servicename.",
                                ConsoleColor.Green);
                            Console.WriteLine(
                                "[U servicename] Uninstall SnapGate Windows NT Service servicename.",
                                ConsoleColor.Green);

                            Console.ReadLine();
                            Environment.Exit(0);
                        }
                    }
                } // if
            }
            catch (NotImplementedException ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    "Error in " + MethodBase.GetCurrentMethod().Name,
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
            }
            catch (Exception ex)
            {
                LogEngine.WriteLog(
                    ConfigurationBag.EngineName,
                    "Error in " + MethodBase.GetCurrentMethod().Name,
                    Constant.LogLevelError,
                    Constant.TaskCategoriesError,
                    ex,
                    Constant.LogLevelError);
                Environment.Exit(0);
            } // try/catch
            finally
            {
                //Spool log queues
                if (LogEngine.QueueAbstractMessage != null)
                {
                    LogEngine.QueueAbstractMessageOnPublish(LogEngine.QueueAbstractMessage.ToArray().ToList());
                }

                if (LogEngine.QueueConsoleMessage != null)
                {
                    LogEngine.QueueConsoleMessageOnPublish(LogEngine.QueueConsoleMessage.ToArray().ToList());
                }
            }
        } // Main

        private static string AskInputLine(string message)
        {
            var ret = string.Empty;
            while (ret == string.Empty)
            {
                Trace.WriteLine(message);
                ret = Console.ReadLine();
            }

            return ret;
        }
    } // Program
} // namespace