using System;
using System.Collections.Generic;
using System.Text;

namespace reCLI.Plugin
{
    public class PluginContext
    {
        public string PluginDirectory
        {
            get; set;
        }

        public PluginMetadata Metadata { get; set; }

        public Guid ID { get; set; }

        /// <summary>
        /// Public APIs for plugin invocation
        /// </summary>
        public IPublicAPI API { get; set; }
    }
}
