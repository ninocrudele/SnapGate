namespace PipelineLaboratory {
    
    
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"PipelineLaboratory.FlatFileSchemaTest", typeof(global::PipelineLaboratory.FlatFileSchemaTest))]
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"PipelineLaboratory.FlatFileSchemaTest", typeof(global::PipelineLaboratory.FlatFileSchemaTest))]
    public sealed class Map1 : global::Microsoft.XLANGs.BaseTypes.TransformBase {
        
        private const string _strMap = @"<?xml version=""1.0"" encoding=""UTF-16""?>
<xsl:stylesheet xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"" xmlns:msxsl=""urn:schemas-microsoft-com:xslt"" xmlns:var=""http://schemas.microsoft.com/BizTalk/2003/var"" exclude-result-prefixes=""msxsl var"" version=""1.0"" xmlns:ns0=""http://PipelineLaboratory.FlatFileSchemaTest"">
  <xsl:output omit-xml-declaration=""yes"" method=""xml"" version=""1.0"" />
  <xsl:template match=""/"">
    <xsl:apply-templates select=""/ns0:Root"" />
  </xsl:template>
  <xsl:template match=""/ns0:Root"">
    <ns0:Root>
      <xsl:for-each select=""Root_Child1"">
        <Root_Child1>
          <Root_Child1_Child1>
            <xsl:value-of select=""Root_Child1_Child4/text()"" />
          </Root_Child1_Child1>
          <Root_Child1_Child2>
            <xsl:value-of select=""Root_Child1_Child3/text()"" />
          </Root_Child1_Child2>
          <Root_Child1_Child3>
            <xsl:value-of select=""Root_Child1_Child2/text()"" />
          </Root_Child1_Child3>
          <Root_Child1_Child4>
            <xsl:value-of select=""Root_Child1_Child1/text()"" />
          </Root_Child1_Child4>
        </Root_Child1>
      </xsl:for-each>
    </ns0:Root>
  </xsl:template>
</xsl:stylesheet>";
        
        private const int _useXSLTransform = 0;
        
        private const string _strArgList = @"<ExtensionObjects />";
        
        private const string _strSrcSchemasList0 = @"PipelineLaboratory.FlatFileSchemaTest";
        
        private const global::PipelineLaboratory.FlatFileSchemaTest _srcSchemaTypeReference0 = null;
        
        private const string _strTrgSchemasList0 = @"PipelineLaboratory.FlatFileSchemaTest";
        
        private const global::PipelineLaboratory.FlatFileSchemaTest _trgSchemaTypeReference0 = null;
        
        public override string XmlContent {
            get {
                return _strMap;
            }
        }
        
        public override int UseXSLTransform {
            get {
                return _useXSLTransform;
            }
        }
        
        public override string XsltArgumentListContent {
            get {
                return _strArgList;
            }
        }
        
        public override string[] SourceSchemas {
            get {
                string[] _SrcSchemas = new string [1];
                _SrcSchemas[0] = @"PipelineLaboratory.FlatFileSchemaTest";
                return _SrcSchemas;
            }
        }
        
        public override string[] TargetSchemas {
            get {
                string[] _TrgSchemas = new string [1];
                _TrgSchemas[0] = @"PipelineLaboratory.FlatFileSchemaTest";
                return _TrgSchemas;
            }
        }
    }
}
