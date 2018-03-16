// ConfigProperties.cs
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
using System;
using System.Xml;
using Microsoft.BizTalk.Component.Interop;

namespace SnapGate.Framework.BizTalk.Adapter.Common
{
    /// <summary>
	/// Summary description for ConfigProperties.
    /// </summary>
    public class ConfigProperties
    {
        // Various useful helper functions
        public static XmlDocument ExtractConfigDomImpl (IPropertyBag pConfig, bool required)
        {
            object obj = null;
            pConfig.Read("AdapterConfig", out obj, 0);
            if (!required && null == obj)
                return null;
            if (null == obj)
                throw new NoAdapterConfig();

            XmlDocument configDom = new XmlDocument();

            string adapterConfig = (string)obj;
            configDom.LoadXml(adapterConfig);

            return configDom;
        }

        public static XmlDocument ExtractConfigDom (IPropertyBag pConfig)
        {
            return ExtractConfigDomImpl(pConfig, true);
        }

        public static XmlDocument IfExistsExtractConfigDom (IPropertyBag pConfig)
        {
            return ExtractConfigDomImpl(pConfig, false);
        }

        public static string ExtractImpl (XmlDocument document, string path, bool required, string alt)
        {
            XmlNode node = document.SelectSingleNode(path);
            if (!required && null == node)
                return alt;
            if (null == node)
                throw new NoSuchProperty(path);
            return node.InnerText;
        }

        public static string IfNotEmptyExtract(XmlDocument document, string path, bool required, string alt)
        {
            XmlNode node = document.SelectSingleNode(path);
            if (!required && (null == node || 0 == node.InnerText.Length) )
                return alt;
            if (null == node)
                throw new NoSuchProperty(path);
            return node.InnerText;
        }

        public static string Extract (XmlDocument document, string path, string alt)
        {
            return ExtractImpl(document, path, true, alt);
        }
        
        public static string IfExistsExtract (XmlDocument document, string path, string alt)
        {
            return ExtractImpl(document, path, false, alt);
        }

        public static int ExtractInt (XmlDocument document, string path)
        {
            string s = Extract(document, path, String.Empty);
            return int.Parse(s);
        }

        public static int IfExistsExtractInt (XmlDocument document, string path, int alt)
        {
            string s = IfExistsExtract(document, path, String.Empty);
            if (0 == s.Length)
                return alt;
            return int.Parse(s);
        }

        public static long ExtractLong (XmlDocument document, string path)
        {
            string s = Extract(document, path, String.Empty);
            return long.Parse(s);
        }

        public static long IfExistsExtractLong (XmlDocument document, string path, long alt)
        {
            string s = IfExistsExtract(document, path, String.Empty);
            if (0 == s.Length)
                return alt;
            return long.Parse(s);
        }

		public static bool ExtractBool(XmlDocument document, string path)
		{
			string s = Extract(document, path, String.Empty);
			return Boolean.Parse(s);
		}

        public static bool IfExistsExtractBool(XmlDocument document, string path, bool alt)
        {
            string s = IfExistsExtract(document, path, String.Empty);
            if (0 == s.Length)
                return alt;
            return Boolean.Parse(s);
        }

        public static long ExtractPollingInterval (XmlDocument document)
        {
            long pollingInterval = ExtractInt(document, "/Config/pollingInterval");
            string pollingUnitOfMeasureStr = Extract(document, "/Config/pollingUnitOfMeasure", "Seconds");

            switch (pollingUnitOfMeasureStr)
            {
                case "Seconds": //  do nothing: seconds is the default
                    break;
                case "Minutes": pollingInterval *= 60;
                    break;
                case "Hours":   pollingInterval *= (60 * 60);
                    break;
                case "Days":    pollingInterval *= (60 * 60 * 24);
                    break;
            }
            return pollingInterval;
        }
    }
}

