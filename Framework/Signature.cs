using HYIS.Framework.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HYIS.Framework
{
    [DataContract]
    internal class Signature:ISignature
    {
        /// <summary>
        /// Name of the assembly
        /// </summary>
        [DataMember]
        public string AssemblyName { get; set; }
        /// <summary>
        /// Name of the class
        /// </summary>
        [DataMember]
        public string ClassName { get; set; }
        /// <summary>
        /// Method parameters
        /// </summary>
        [DataMember]
        public List<Parameter> Parameters { get; set; }
    }
}
