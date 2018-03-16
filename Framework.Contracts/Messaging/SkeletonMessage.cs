// --------------------------------------------------------------------------------------------------
// <copyright file = "SkeletonMessage.cs" company="GrabCaster Ltd">
//   Copyright (c) 2013 - 2016 GrabCaster Ltd All Rights Reserved.
// </copyright>
// <summary>
//    Author: Nino Crudele
//    Blog:   http://ninocrudele.me
// 
//    Unless explicitly acquired and licensed from Licensor under another
//    license, the contents of this file are subject to the Reciprocal Public
//    License ("RPL") Version 1.5, or subsequent versions as allowed by the RPL,
//    and You may not copy or use this file in either source code or executable
//    form, except in compliance with the terms and conditions of the RPL.
//    
//    All software distributed under the RPL is provided strictly on an "AS
//    IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, AND
//    LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT
//    LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//    PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the RPL for specific
//    language governing rights and limitations under the RPL. 
// 
//    Reciprocal Public License 1.5 (RPL1.5) license is described here: 
//    http://www.opensource.org/licenses/rpl1.5.txt
//  </summary>
// --------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrabCaster.Framework.Contracts.Bubbling;

namespace GrabCaster.Framework.Contracts.Messaging
{
    using System.Runtime.Serialization;

    [Serializable]
    public class SkeletonMessage: ISkeletonMessage
    {
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public IDictionary<string, string> Properties { get; set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletonMessage"/> class.
        /// </summary>
        /// <param name="body">
        /// The body.
        /// </param>
        public SkeletonMessage(byte[] body)
        {
            Properties = new Dictionary<string, string>();
            Body = body;
        }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>
        /// The serialize message.
        /// </summary>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static byte[] SerializeMessage(BubblingObject bubblingObject)
        {
            return GrabCaster.Framework.Serialization.Object.SerializationEngine.ObjectToByteArray(bubblingObject);
        }

        /// <summary>
        /// The serialize message.
        /// </summary>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static BubblingObject DeserializeMessage(byte[] byteArray)
        {
            return (BubblingObject) GrabCaster.Framework.Serialization.Object.SerializationEngine.ByteArrayToObject(byteArray);
        }

    }
}
