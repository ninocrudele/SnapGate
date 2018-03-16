#region License
//-----------------------------------------------------------------------
// <copyright file="EventHubsCheckPointPattern.cs" company="Antonino Crudele">
//   Copyright (c) Antonino Crudele. All Rights Reserved.
// </copyright>
// <license>
//   This work is registered with the UK Copyright Service.
//   Registration No:284695248
//   Licensed under the Reciprocal Public License 1.5 (RPL1.5) 
//   See License.txt in the project root for license information.
// </license>
//-----------------------------------------------------------------------
#endregion

namespace Framework.Base
{
    /// <summary>
    /// Types of patterns for check point for event hubs.
    /// </summary>
    public enum EventHubsCheckPointPattern
    {
        /// <summary>
        /// The last receiving check point.
        /// </summary>
        CheckPoint,

        /// <summary>
        /// The configured starting Date Time receiving.
        /// </summary>
        DT,

        /// <summary>
        /// The configured starting date time receiving and epoch.
        /// </summary>
        DTEPOCH,

        /// <summary>
        /// The current UTC date time.
        /// </summary>
        DTUTCNOW,

        /// <summary>
        /// The current date time.
        /// </summary>
        DTNOW,

        /// <summary>
        /// The current UTC date time and configured epoch.
        /// </summary>
        DTUTCNOWEPOCH,

        /// <summary>
        /// The current date time and configured epoch.
        /// </summary>
        DTNOWEPOCH
    } // EventHubsCheckPointPattern
} // namespace