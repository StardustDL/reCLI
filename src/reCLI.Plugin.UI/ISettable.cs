using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace reCLI.Plugin.UI
{
    public interface ISettable : IFeatures
    {
        UIElement SettingPage { get; }
    }
}
