using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HYIS.Framework.Contracts
{
    [DataContract]
    public class EventMessage : IEventMessage
    {
        public EventMessage(string MessageID, List<EventAction> EventActions)
        {
            this.MessageID = MessageID;
            this.EventActions = EventActions;
        }
        [DataMember]
        public string MessageID { get; set; }
        [DataMember]
        public List<EventAction> EventActions { get; set; }
    }
}
