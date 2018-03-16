using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HYIS.Framework.Contracts
{
    [DataContract]
    public class Parameter : IParameter
    {
        public Parameter(string Name,
                        ParameterType Type,
                        object Value)
        {
            this.Name = Name;
            this.Type = Type;
            this.Value = Value;

        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public ParameterType Type { get; set; }
        [DataMember]
        public object Value { get; set; }
    }
}
