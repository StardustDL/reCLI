using reCLI.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace reCLI.Plugin
{
    public class PluginMetadata : NotifyPropertyChangedModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }

        public string ExecuteFileName { get; set; }

        public string[] PluginTypes { get; set; }

        public override string ToString() => Name;
    }
}
