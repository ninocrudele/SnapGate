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

using System.ServiceProcess;

#endregion

namespace SnapGate.Framework.NTService
{
    /// <summary>
    ///     Properties for the Windows Service.
    /// </summary>
    internal static class NtSharedProperties
    {
        /// <summary>
        ///     Gets or sets the account for the Windows Service.
        /// </summary>
        /// <value>
        ///     The account for the Windows Service.
        /// </value>
        public static ServiceAccount Account { get; set; }

        /// <summary>
        ///     Gets or sets the password for the Windows Service.
        /// </summary>
        /// <value>
        ///     The password for the Windows Service.
        /// </value>
        public static string Password { get; set; }

        /// <summary>
        ///     Gets or sets the username for the Windows Service.
        /// </summary>
        /// <value>
        ///     The username for the Windows Service.
        /// </value>
        public static string Username { get; set; }

        /// <summary>
        ///     Gets or sets the name for the Windows Service.
        /// </summary>
        /// <value>
        ///     The name for the Windows Service.
        /// </value>
        public static string WindowsNtServiceName { get; set; }

        /// <summary>
        ///     Gets or sets the display name for the Windows Service.
        /// </summary>
        /// <value>
        ///     The display name for the Windows Service.
        /// </value>
        public static string DisplayName { get; set; }

        /// <summary>
        ///     Gets or sets the description for the Windows Service.
        /// </summary>
        /// <value>
        ///     The description for the Windows Service.
        /// </value>
        public static string Description { get; set; }
    } // NTSharedProperties
} // namespace