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
using System.Runtime.Serialization.Formatters.Binary;
using SnapGate.Framework.Base;

#endregion

namespace SnapGate.Framework.Serialization.Object
{
    /// <summary>
    ///     Serialization engine class
    /// </summary>
    public static class SerializationEngine
    {
        /// <summary>
        ///     serialize object to Array
        /// </summary>
        /// <param name="objectData">
        ///     Object to serialize
        /// </param>
        /// <returns>
        ///     Return a byte array
        /// </returns>
        public static byte[] ObjectToByteArray(object objectData)
        {
            if (objectData == null)
            {
                return null;
            }

            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, objectData);
            return memoryStream.ToArray();
        }

        /// <summary>
        ///     Serialize Array to Object
        /// </summary>
        /// <param name="arrayBytes">
        ///     Array byte to deserialize
        /// </param>
        /// <returns>
        ///     Object deserialized
        /// </returns>
        public static object ByteArrayToObject(byte[] arrayBytes)
        {
            if (arrayBytes == null)
            {
                return EncodingDecoding.EncodingString2Bytes(string.Empty);
            }

            var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();
            memoryStream.Write(arrayBytes, 0, arrayBytes.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var obj = binaryFormatter.Deserialize(memoryStream);
            return obj;
        }
    }
}