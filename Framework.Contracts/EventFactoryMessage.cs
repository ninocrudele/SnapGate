using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HYIS.Framework.Messaging;

namespace HYIS.Framework.Factory
{


    /// <summary>
    /// EventMessage class
    /// </summary>
    public class EventFactoryMessage
    {
        private EventMessage eventMessage;

        public EventFactoryMessage(string messageID, List<Consumer> Consumers)
        {
            
            EventMessage.Consumer [] _consumers = new EventMessage.Consumer[Consumers.Count];

        }
        /// <summary>
        /// Unique ID
        /// </summary>
        public string MessageID { get; set; }

        /// <summary>
        /// Consumers in messages
        /// </summary>
        public Consumer[] Consumers { get; set; }

        /// <summary>
        /// Consumer class
        /// </summary>
        public class Consumer
        {
            public Consumer(string consumerID, string name, string description, List<EventAction> eventAction)
            {
                ConsumerID = consumerID;
                Name = name;
                Description = description;
                Actions = eventAction;
            }
            public string ConsumerID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            // PartitionID correlated is in table storage and locally
            public EventAction[] Actions { get; set; }

            /// <summary>
            /// EventAction class 
            /// </summary>
            public class EventAction
            {

                public EventAction(string actionID,
                            string name,
                            string description,
                            string signature,
                            Parameter[] parameters,
                            string data,
                            bool returnValueRequired,
                            Correlation[] correlations)
                {

                }
                /// <summary>
                /// Unique Action ID
                /// </summary>
                public string ActionID { get; set; }
                /// <summary>
                /// Method name
                /// </summary>
                public string Name { get; set; }
                /// <summary>
                /// Description
                /// </summary>
                public string Description { get; set; }
                /// <summary>
                /// the signature [assname|classname|method] is in Azute table and local sincronized
                /// </summary>
                public string Signature { get; set; }
                /// <summary>
                /// Method parameters
                /// </summary>
                public Parameter[] Parameters { get; set; }
                /// <summary>
                /// Data to pass in method
                /// </summary>
                public string Data { get; set; }
                /// <summary>
                /// If return data required
                /// </summary>
                public bool ReturnValueRequired { get; set; }
                /// <summary>
                /// If >0 means tha there are correlation to satified
                /// </summary>
                public Correlation[] Correlations { get; set; }


                /// <summary>
                /// Parameter class
                /// </summary>
                [DataContract]
                public class Parameter
                {
                    public Parameter(string name,
                                object value)
                    {
                        Name = name;
                        Value = value;
                    }
                    //Public enums
                    public enum ParameterType
                    {
                        String,
                        Int
                    }
                    public string Name { get; set; }
                    public ParameterType Type { get; set; }
                    public object Value { get; set; }
                } /// Parameter class

                /// <summary>
                /// Correlation class
                /// </summary>
                [DataContract]
                public class Correlation
                {
                    public Correlation(string name,
                                string description,
                                string correlationToken,
                                Consumer[] consumers)
                    {


                    }
                    /// <summary>
                    /// Nmae
                    /// </summary>
                    public string Name { get; set; }
                    /// <summary>
                    /// Description
                    /// </summary>
                    public string Description { get; set; }
                    /// <summary>
                    /// Token to correlate, if empty alwaus execute
                    /// </summary>
                    public string CorrelationToken { get; set; }

                    /// <summary>
                    /// Consumers to call if correlation token condition is satified, 
                    /// if empty so always othewise use xpath in data resul or regular expression
                    /// </summary>
                    public Consumer[] Consumers { get; set; }

                }


            }/// EventAction class 
    
        }/// Consumer class



    }

    



}
