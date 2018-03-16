using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SnapGate.Framework.Engine
{
    public enum ComponentDataType { AssemblyRoot, AssemblyTrigger, AssemblyEvent, AssemblyComponent,ConfigurationTrigger, ConfigurationEvent, ConfigurationChain, ConfigurationComponent }

    public class ComponentData
    {
        public ComponentDataType FileType { get; set; }
        public Byte[] Content { get; set; }

    }
}

