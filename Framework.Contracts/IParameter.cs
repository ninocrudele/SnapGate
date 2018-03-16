using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HYIS.Framework.Contracts
{

    //Public enums
    public enum ParameterType
    {
        String,
        Int
    }

    /// <summary>
    /// Parameter class
    /// </summary>
    public interface IParameter
    {

        string Name { get; set; }
        ParameterType Type { get; set; }
        object Value { get; set; }
    } /// Parameter class

}