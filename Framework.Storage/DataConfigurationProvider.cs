
namespace GrabCaster.Framework.Storage
{
    using System.IO;
    using System.Text.RegularExpressions;
    using GrabCaster.Framework.Base;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The data configuration provider, get all data GC needs.
    /// </summary>
    public static class DataConfigurationProvider
    {

        /// <summary>
        /// The file system get triggers.
        /// </summary>
        /// <param name="dataConfigurationProviderType">
        /// The data configuration provider type.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static Dictionary<string,Assembly> FileSystemGetTriggers(DataConfigurationProviderType dataConfigurationProviderType)
        {
            Dictionary<string, Assembly> listData = null;
            switch (dataConfigurationProviderType)
            {
                case DataConfigurationProviderType.FileSystem:
                    var triggersDirectory = Configuration.DirectoryTriggers();
                    var regTriggers = new Regex(Configuration.TriggersDllExtension);
                    List<string> assemblyFilesTriggers = Directory.GetFiles(triggersDirectory, Configuration.TriggersDllExtensionLookFor)
                                                .Where(path => regTriggers.IsMatch(path))
                                                .ToList();
                    foreach (var assemblyFile in assemblyFilesTriggers)
                    {
                        
                        listData.Add(assemblyFile, Assembly.LoadFrom(assemblyFile));
                    }
                    break;
                case DataConfigurationProviderType.Azure:
                    break;
                default:
                    break;
            }

            return list;
        }

        public static List<string> FileSystemGetEvents(DataConfigurationProviderType dataConfigurationProviderType)
        {
            List<string> list = null;
            switch (dataConfigurationProviderType)
            {
                case DataConfigurationProviderType.FileSystem:
                    var eventsDirectory = Configuration.DirectoryEvents();
                    var regEvents = new Regex(Configuration.EventsDllExtension);
                    var assemblyFilesEvents =
                        Directory.GetFiles(eventsDirectory, Configuration.EventsDllExtensionLookFor)
                            .Where(path => regEvents.IsMatch(path))
                            .ToList();
                    break;
                case DataConfigurationProviderType.Azure:
                    break;
                default:
                    break;
            }

            return list;
        }

    }
}
