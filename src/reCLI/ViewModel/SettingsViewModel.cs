using reCLI.Core;
using reCLI.Plugin;
using reCLI.Plugin.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace reCLI.ViewModel
{
    public class SettingsViewModel : NotifyPropertyChangedModel
    {
        public ObservableCollection<IPlugin> Plugins { get; set; }

        public static SettingsViewModel Current { get; set; }

        public SettingsViewModel()
        {
            Current = this;
            Plugins = new ObservableCollection<IPlugin>();
            foreach (var v in App.PluginManager.GetPluginsForFeature<ISettable>())
            {
                Plugins.Add((IPlugin)v.Item1);
            }
        }
    }
}
