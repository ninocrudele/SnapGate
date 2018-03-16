using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HYIS.Framework.Contracts
{
    /// <summary>
    /// Correlation class
    /// </summary>
    public interface ICorrelation
    {

        /// <summary>
        /// Nmae
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// Token to correlate, if empty alwaus execute
        /// </summary>
        string CorrelationToken { get; set; }
        /// <summary>
        /// Consumers to call if correlation token condition is satified, 
        /// if empty so always othewise use xpath in data resul or regular expression
        /// </summary>
        List<IEventAction> EventActions { get; set; }

    }

}

    

