using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HYIS.Framework.Contracts
{
    [DataContract]
    public class Correlation : ICorrelation
    {
        public Correlation(string Name,
                            string Description,
                            string CorrelationToken,
                            List<IEventAction> EventActions)
        {
            this.Name = Name;
            this.Description = Description;
            this.CorrelationToken = CorrelationToken;
            this.EventActions = EventActions;
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string CorrelationToken { get; set; }
        [DataMember]
        public List<IEventAction> EventActions { get; set; }
    }
}
