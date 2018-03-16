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

namespace SnapGate.Framework.Base
{
    /// <summary>
    ///     Holds the constants.
    /// </summary>
    public static class Constant
    {
        /// <summary>
        ///     The ID for the high critical events (Engine).
        /// </summary>
        public const int LogLevelError = 1;

        public const int LogLevelWarning = 2;
        public const int LogLevelInformation = 3;
        public const int LogLevelVerbose = 4;


        /// <summary>
        ///     The task category error.
        /// </summary>
        public static string TaskCategoriesError { get; } = ConfigurationBag.EngineName;

        /// <summary>
        ///     The task category for console.
        /// </summary>
        public static string TaskCategoriesConsole { get; } = "Console";

        public static string EmbeddedEventId { get; } = "{A31209D7-C989-4E5D-93DA-BD341D843870}";

        /// <summary>
        ///     The task category for event hubs.
        /// </summary>
        public static string TaskCategoriesEventHubs { get; } = "Event Hub";
    } // Constant
} // namespace