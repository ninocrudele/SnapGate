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
using System.Text;

#endregion

namespace SnapGate.Framework.Base
{
    public enum EncodingType
    {
        Utf8,
        Ascii,
        BigEndianUnicode,
        Default,
        Utf32,
        Utf7,
        Unicode
    }

    public static class EncodingDecoding
    {
        /// <summary>
        ///     The last error point
        /// </summary>
        public static string EncodingBytes2String(byte[] value)
        {
            EncodingType encodingType = ConfigurationBag.Configuration.EncodingType;
            switch (encodingType)
            {
                case EncodingType.Utf8:
                    return Encoding.UTF8.GetString(value);
                case EncodingType.Ascii:
                    return Encoding.ASCII.GetString(value);
                case EncodingType.BigEndianUnicode:
                    return Encoding.BigEndianUnicode.GetString(value);
                case EncodingType.Default:
                    return Encoding.Default.GetString(value);
                case EncodingType.Utf32:
                    return Encoding.UTF32.GetString(value);
                case EncodingType.Utf7:
                    return Encoding.UTF7.GetString(value);
                case EncodingType.Unicode:
                    return Encoding.Unicode.GetString(value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(encodingType), encodingType, null);
            }
        }

        /// <summary>
        ///     The last error point
        /// </summary>
        public static byte[] EncodingString2Bytes(string value)
        {
            EncodingType encodingType = ConfigurationBag.Configuration.EncodingType;
            switch (encodingType)
            {
                case EncodingType.Utf8:
                    return Encoding.UTF8.GetBytes(value);
                case EncodingType.Ascii:
                    return Encoding.ASCII.GetBytes(value);
                case EncodingType.BigEndianUnicode:
                    return Encoding.BigEndianUnicode.GetBytes(value);
                case EncodingType.Default:
                    return Encoding.Default.GetBytes(value);
                case EncodingType.Utf32:
                    return Encoding.UTF32.GetBytes(value);
                case EncodingType.Utf7:
                    return Encoding.UTF7.GetBytes(value);
                case EncodingType.Unicode:
                    return Encoding.Unicode.GetBytes(value);
                default:
                    throw new ArgumentOutOfRangeException(nameof(encodingType), encodingType, null);
            }
        }
    }
}