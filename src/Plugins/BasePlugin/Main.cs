using reCLI.Core;
using reCLI.Plugin;
using reCLI.Plugin.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace reCLI.Plugins.{pluginName}
{
    public class Main : IPluginWithIcon,IGlobalQuery
    {
        PluginContext context;

        public Guid ID { get => context.ID; }

        public string Name { get => "{pluginName}"; }

        public string Description { get => "{pluginDes}"; }

        public string Keyword => "{pluginKeyword}";

        ImageSource icon;

        public ImageSource Icon => icon;

        Random rnd = new Random();

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                icon = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/{pluginIconName}.png")));
                icon.Freeze();
                return true;
            });
        }

        public Task<IEnumerable<Answer>> Query(Query query)
        {
            IEnumerable<Answer> iter()
            {
                
            }

            return Task.Run(() => iter());
        }
        public Task Uninitialize()
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Answer>> GlobalQuery(Query query)
        {
            throw new NotImplementedException();
        }
    }
}
