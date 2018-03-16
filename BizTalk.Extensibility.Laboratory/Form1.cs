// Form1.cs
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
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using SnapGate.Framework.Base;

#endregion

namespace SnapGate.BizTalk.Extensibility.Laboratory
{
    public partial class Form1 : Form
    {
        public string prpassdirectory;
        public string prpassemblyfile;
        public string prpfilename;
        public string prpinputinstance;
        public string prppipetype;

        public Form1()
        {
            InitializeComponent();
        }


        private void buttonTry_Click(object sender, EventArgs e)
        {
            prpassemblyfile =
                @"C:\Users\Nino\Documents\SnapGate\SnapGate.Framework\BizTalk.Laoratory\PipelineLaboratory\PipelineLaboratory\bin\Debug\PipelineLaboratory.dll";
            prppipetype = "PipelineLaboratory.ReceivePipelineFlat";
            prpinputinstance =
                @"C:\Users\Nino\Documents\SnapGate\SnapGate.Framework\BizTalk.Laoratory\PipelineLaboratory\PipelineLaboratory\TestFile.txt";
            prpfilename =
                @"C:\Users\Nino\Documents\SnapGate\SnapGate.Framework\BizTalk.Laoratory\PipelineLaboratory\PipelineLaboratory\ReceivePipelineFlat.btp";
            byte[] content = BizTalkPipelines.ExecutePipeline(prpassemblyfile, prpassemblyfile, prppipetype,
                prpinputinstance, prpfilename);
            string s = EncodingDecoding.EncodingBytes2String(content);
            MessageBox.Show(s);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string inputinstance = File.ReadAllText("");
            //IBaseMessage inputMessage = MessageHelper.CreateFromStream(BizTalkPipelines.LoadMessage(inputinstance));
            //if (inputMessage == null)
            //    throw new ArgumentNullException("pInMsg");
            //TransformBase transform = BizTalkTransform.CreateMapInstance();
            //Stream newMessageData = BizTalkTransform.MapMessage(inputMessage.BodyPart.GetOriginalDataStream(), transform);

            //StreamReader reader = new StreamReader(newMessageData);
            //string contentRp = reader.ReadToEnd();

            //byte[] returnContent = System.Text.EncodingDecoding.EncodingString2Bytes(contentRp);
            string s = LoadInternalArtifactData(
                @"C:\Users\Nino\Documents\SnapGate\SnapGate.Framework\BizTalk.Laoratory\PipelineLaboratory\PipelineLaboratory\bin\Debug\PipelineLaboratory.dll",
                "PipelineLaboratory.Map1",
                true);
        }

        private string LoadInternalArtifactData(string parentAssemblyName, string artypename, bool isasslocation)
        {
            try
            {
                Assembly asm = Assembly.LoadFrom(parentAssemblyName);

                Type t = asm.GetType(artypename);
                if (t == null)
                    return artypename + " no found in assembly " + parentAssemblyName;
                object o = asm.CreateInstance(artypename);


                PropertyInfo pi = o.GetType()
                    .GetProperty("XmlContent", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
                object viewData = pi.GetValue(o, null);
                string tmpViewData = viewData != null ? viewData.ToString() : string.Empty;
                string dataXslt = tmpViewData.Replace("utf-16", "utf-8");

                XslCompiledTransform proc = new XslCompiledTransform();

                using (StringReader sr = new StringReader(dataXslt))
                {
                    using (XmlReader xr = XmlReader.Create(sr))
                    {
                        proc.Load(xr);
                    }
                }

                string resultXML;

                string xmlData = File.ReadAllText("c:\\demoxml.txt");
                using (StringReader sr = new StringReader(xmlData))
                {
                    using (XmlReader xr = XmlReader.Create(sr))
                    {
                        using (StringWriter sw = new StringWriter())
                        {
                            proc.Transform(xr, null, sw);
                            resultXML = sw.ToString();
                        }
                    }
                }

                return resultXML;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}