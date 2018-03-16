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

using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using SnapGate.Framework.Base;

#endregion

namespace SnapGate.Framework.Contracts.Serialization
{
    /// <summary>
    ///     Serialization engine class
    /// </summary>
    public static class Serialization
    {
        /// <summary>
        ///     The object to byte array no compressed.
        /// </summary>
        /// <param name="objectData">
        ///     The object data.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        public static byte[] ObjectToByteArrayNoCompressed(object objectData)
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
        ///     The byte array to objectold.
        /// </summary>
        /// <param name="bytesArray">
        ///     The bytes array.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public static object ByteArrayToObjectold(byte[] bytesArray)
        {
            if (bytesArray == null)
            {
                return EncodingDecoding.EncodingString2Bytes(string.Empty);
            }

            var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();
            memoryStream.Write(bytesArray, 0, bytesArray.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var obj = binaryFormatter.Deserialize(memoryStream);
            return obj;
        }

        /// <summary>
        ///     The object to byte array.
        /// </summary>
        /// <param name="objectData">
        ///     The object data.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public static byte[] ObjectToByteArray(object objectData)
        {
            var objStream = new MemoryStream();
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();

            bf.Serialize(ms, objectData);

            var objDeflated = new DeflateStream(objStream, CompressionMode.Compress);

            objDeflated.Write(ms.GetBuffer(), 0, (int) ms.Length);
            objDeflated.Flush();
            objDeflated.Close();

            return objStream.ToArray();
        }

        /// <summary>
        ///     The byte array to object.
        /// </summary>
        /// <param name="byteArray">
        ///     The byte array.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        public static object ByteArrayToObject(byte[] byteArray)
        {
            var inMs = new MemoryStream(byteArray);
            inMs.Seek(0, 0);
            var zipStream = new DeflateStream(inMs, CompressionMode.Decompress, true);

            var outByt = ReadFullStream(zipStream);
            zipStream.Flush();
            zipStream.Close();

            var outMs = new MemoryStream(outByt);
            outMs.Seek(0, 0);

            var bf = new BinaryFormatter();

            object outObject = (DataTable) bf.Deserialize(outMs, null);

            return outObject;
        }

        /// <summary>
        ///     The data table to byte array.
        /// </summary>
        /// <param name="dataTable">
        ///     The data table.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        public static byte[] DataTableToByteArray(DataTable dataTable)
        {
            var objStream = new MemoryStream();
            dataTable.RemotingFormat = SerializationFormat.Binary;
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();

            bf.Serialize(ms, dataTable);

            var objDeflated = new DeflateStream(objStream, CompressionMode.Compress);

            objDeflated.Write(ms.GetBuffer(), 0, (int) ms.Length);
            objDeflated.Flush();
            objDeflated.Close();

            return objStream.ToArray();
        }

        /// <summary>
        ///     The byte array to data table.
        /// </summary>
        /// <param name="byteDataTable">
        ///     The byte data table.
        /// </param>
        /// <returns>
        ///     The <see cref="DataTable" />.
        /// </returns>
        public static DataTable ByteArrayToDataTable(byte[] byteDataTable)
        {
            var outDs = new DataTable();

            var inMs = new MemoryStream(byteDataTable);
            inMs.Seek(0, 0);
            var zipStream = new DeflateStream(inMs, CompressionMode.Decompress, true);

            var outByt = ReadFullStream(zipStream);
            zipStream.Flush();
            zipStream.Close();

            var outMs = new MemoryStream(outByt);
            outMs.Seek(0, 0);
            outDs.RemotingFormat = SerializationFormat.Binary;
            var bf = new BinaryFormatter();

            outDs = (DataTable) bf.Deserialize(outMs, null);

            return outDs;
        }

        /// <summary>
        ///     The read full stream.
        /// </summary>
        /// <param name="stream">
        ///     The stream.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>byte[]</cref>
        ///     </see>
        ///     .
        /// </returns>
        private static byte[] ReadFullStream(Stream stream)
        {
            var buffer = new byte[32768];

            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    var read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        return ms.ToArray();
                    }

                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}