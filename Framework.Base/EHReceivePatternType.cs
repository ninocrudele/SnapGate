#region License
//-----------------------------------------------------------------------
// <copyright file="EHReceivePatternType.cs" company="Antonino Crudele">
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
    /// Types of event hubs patterns.
    /// </summary>
    public enum EHReceivePatternType
    {
        /// <summary>
        /// The pattern type is direct.
        /// </summary>
        Direct,

        /// <summary>
        /// The pattern type is abstract.
        /// </summary>
        Abstract
    } // EHReceivePatternType
} // namespace