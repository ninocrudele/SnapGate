// InstConfigPropertyBag.cs
// 
// BSD 3-Clause License
// 
// Copyright (c) 2014-2016, Nino Crudele <nino dot crudele at live dot com>
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// * Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
#region Usings

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.BizTalk.Component.Interop;

#endregion

namespace SnapGate.BizTalk.Extensibility
{
    /// <summary>
    ///     IPropertyBag implementation for per-instance
    ///     pipeline config
    /// </summary>
    /// <example><![CDATA[
    /// <Properties>
    ///   <AddXmlDeclaration vt="11">0</AddXmlDeclaration>
    ///   <PreserveBom vt="11">0</PreserveBom>
    /// </Properties>
    /// ]]></example>
    public class InstConfigPropertyBag : IPropertyBag
    {
        private Hashtable _values = new Hashtable();

        /// <summary>
        ///     Creates a new instance from the config XML
        /// </summary>
        /// <param name="reader">The Xml reader to read from</param>
        public InstConfigPropertyBag(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    String name = reader.LocalName;
                    String type = reader.GetAttribute("vt");
                    String value = reader.ReadString();
                    Write(name, type, value);
                }
            }
        }

        void IPropertyBag.Read(string propName, out object ptrVar, int errorLog)
        {
            ptrVar = _values[propName];
        }

        void IPropertyBag.Write(string propName, ref object ptrVar)
        {
            _values[propName] = ptrVar;
        }

        /// <summary>
        ///     Helper function to read an property (used by tests)
        /// </summary>
        /// <param name="propname">Name of the property to read</param>
        /// <returns>The property value, or null if not found</returns>
        public object Read(string propname)
        {
            object ret = null;
            ((IPropertyBag) this).Read(propname, out ret, 1);
            return ret;
        }

        private void Write(string name, string typeCode, string value)
        {
            VarEnum varKind = (VarEnum) Enum.Parse(typeof(VarEnum), typeCode);
            object realVal = ToType(varKind, value);
            ((IPropertyBag) this).Write(name, ref realVal);
        }

        private object ToType(VarEnum type, string value)
        {
            switch (type)
            {
                case VarEnum.VT_BOOL:
                    return XmlConvert.ToBoolean(value);
                case VarEnum.VT_DATE:
                    return XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified);
                case VarEnum.VT_DECIMAL:
                    return XmlConvert.ToDecimal(value);
                case VarEnum.VT_EMPTY:
                    return null;
                case VarEnum.VT_I1:
                    return XmlConvert.ToSByte(value);
                case VarEnum.VT_I2:
                    return XmlConvert.ToInt16(value);
                case VarEnum.VT_INT:
                case VarEnum.VT_I4:
                    return XmlConvert.ToInt32(value);
                case VarEnum.VT_I8:
                    return XmlConvert.ToInt64(value);
                case VarEnum.VT_R4:
                    return XmlConvert.ToSingle(value);
                case VarEnum.VT_R8:
                    return XmlConvert.ToDouble(value);
                case VarEnum.VT_UI1:
                    return XmlConvert.ToByte(value);
                case VarEnum.VT_UI2:
                    return XmlConvert.ToUInt16(value);
                case VarEnum.VT_UINT:
                case VarEnum.VT_UI4:
                    return XmlConvert.ToUInt32(value);
                case VarEnum.VT_UI8:
                    return XmlConvert.ToUInt64(value);
                default:
                    return value;
            }
        }
    }
}