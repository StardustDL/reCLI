using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace reCLI.Plugin.UI
{
    public interface IPluginWithIcon : IPlugin
    {
        ImageSource Icon { get; }
    }
}
