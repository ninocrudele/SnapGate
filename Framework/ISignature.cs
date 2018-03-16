using HYIS.Framework.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HYIS.Framework
{
    public interface ISignature
    {
        /// <summary>
        /// Name of the assembly
        /// </summary>
        string AssemblyName { get; set; }
        /// <summary>
        /// Name of the class
        /// </summary>
        string ClassName { get; set; }
        /// <summary>
        /// Method parameters
        /// </summary>
       // List<Parameter> Parameters { get; set; }
    }
}
