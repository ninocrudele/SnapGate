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

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SnapGate.Framework.Base;
using SnapGate.Framework.Deployment;

#endregion

namespace ConsoleApplicationLab
{
    class Program
    {
        static void Main(string[] args)
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

            foreach (var file in deployFiles)
            {
                string projectName = Path.GetFileNameWithoutExtension(publishingFolder + file);
                string projectType = Path.GetExtension(publishingFolder + file).Replace(".", "");
                Jit.CompilePublishing(projectType, projectName, "Release", "AnyCpu");
            }
        }
    }
}