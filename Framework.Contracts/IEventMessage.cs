using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HYIS.Framework.Contracts
{

    /// <summary>
    /// EventMessage class
    /// </summary>
    public interface IEventMessage : IBaseMessage
    {

        /// <summary>
        /// Unique ID
        /// </summary>
        string MessageID { get; set; }
        /// <summary>
        /// Consumers in messages
        /// </summary>
        List<EventAction> EventActions { get; set; }

    }
}