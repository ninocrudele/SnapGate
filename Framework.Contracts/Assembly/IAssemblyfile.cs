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
using System.Reflection;
using SnapGate.Framework.Contracts.Bubbling;

#endregion

namespace SnapGate.Framework.Contracts.AssemblyFile
{
    public interface IAssemblyfile
    {
        string Id { get; set; }

        string Name { get; set; }
        string Description { get; set; }
        string Shared { get; set; }
        string PollingRequired { get; set; }
        string Nop { get; set; }
        Version Version { get; set; }

        /// <summary>
        ///     Gets or sets the assembly content.
        /// </summary>
        byte[] AssemblyContent { get; set; }

        /// <summary>
        ///     Assembly object ready to invoke (performances)
        /// </summary>
        Assembly AssemblyObject { get; set; }

        /// <summary>
        ///     Internal class type to invoke
        /// </summary>
        Type AssemblyClassType { get; set; }

        /// <summary>
        ///     Gets or sets the assembly file.
        /// </summary>
        string AssemblyFile { get; set; }

        /// <summary>
        ///     Gets or sets the base actions.
        /// </summary>
        List<BaseAction> BaseActions { get; set; }

        Dictionary<string, Property> Properties { get; set; }
    }
}