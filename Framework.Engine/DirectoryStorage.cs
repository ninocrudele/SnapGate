using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrabCaster.Framework.Contracts.Configuration
{
    using System.Runtime.Serialization;

    using GrabCaster.Framework.Contracts.Bubbling;
    using GrabCaster.Framework.Engine;

    [Serializable,DataContract]
    public class DirectoryStorage
    {
        public List<SyncConfigurationFile> SyncConfigurationFileList { get; set; }
        public List<TriggerConfiguration> TriggerConfigurationList { get; set; }
        public List<EventConfiguration> EventConfigurationList { get; set; }
        public List<BubblingEvent> BubblingTriggersEventsActive { get; set; }
        public List<BubblingEvent> BubblingTriggerConfigurationsPolling { get; set; }
        public List<BubblingEvent> BubblingTriggerConfigurationsSingleInstance { get; set; }
    }
}
