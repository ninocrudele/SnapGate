// AdapterManagementBase.cs
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
using System.IO;
using System.Xml;
using System.Resources;
using System.Reflection;
using Microsoft.Win32;

namespace SnapGate.Framework.BizTalk.Adapter.Common
{
	public class AdapterManagementBase
	{
		//  implementation - access schema buried in resources

		protected string GetSchemaFromResource (string name)
		{
			Assembly assem = this.GetType().Assembly;
            using (Stream stream = assem.GetManifestResourceStream(name))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string schema = reader.ReadToEnd();
                    return schema;
                }
            }
		}

		protected XmlDocument LocalizeSchemaDOM (string schema, ResourceManager resourceManager)
		{
			XmlDocument document = new XmlDocument();
			document.LoadXml(schema);

			XmlNodeList nodes = document.SelectNodes("/descendant::*[@_locID]");
			foreach (XmlNode node in nodes)
			{
				string locID = node.Attributes["_locID"].Value;
				node.InnerText = resourceManager.GetString(locID);
			}

			return document;
		}

		protected string MakeString (XmlDocument document)
		{
            using (StringWriter writer = new StringWriter())
            {
                document.WriteTo(new XmlTextWriter(writer));
                return writer.ToString();
            }
		}

		protected string LocalizeSchema (string schema, ResourceManager resourceManager)
		{
			return MakeString(LocalizeSchemaDOM(schema, resourceManager));
		}

		protected XmlDocument AddPathToEditorAssembly (XmlDocument document, string assemblyPath)
		{
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
			nsmgr.AddNamespace("baf", "BiztalkAdapterFramework.xsd");

			//  add editor and converter elements that have an assembly attribute
			string xpath = "/descendant::baf:editor[@assembly] | /descendant::baf:converter[@assembly]";

			XmlNodeList nodes = document.SelectNodes(xpath, nsmgr);
			foreach (XmlNode node in nodes)
			{
				XmlAttribute attribute = node.Attributes["assembly"];
				attribute.Value = assemblyPath + attribute.Value;
			}

			return document;
		}

		//  useful debug code
		protected string GetSchemaFromFile (string name)
		{
            using (RegistryKey bts30 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\BizTalk Server\\3.0"))
            {
                string installPath = (string)bts30.GetValue("InstallPath");
                string productLanguage = (string)bts30.GetValue("ProductLanguage");
                string fullName = installPath + productLanguage + "\\" + name;

                using (StreamReader reader = new StreamReader(fullName))
                {
                    string schema = reader.ReadToEnd();
                    return schema;
                }
            }
		}
	}
}