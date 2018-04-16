using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reCLI.Plugins.FileSearch
{
    public class Settings : reCLI.Core.NotifyPropertyChangedModel
    {
        public static Settings Current { get; set; }

        public ObservableCollection<string> Paths { get; set; }
    }
}
