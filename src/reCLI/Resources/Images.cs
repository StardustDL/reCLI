using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace reCLI.Resources
{
    public static class Images
    {
        public static ImageSource App { get; private set; }
        public static ImageSource Plugin { get; private set; }
        public static ImageSource AllPlugins { get; private set; }
        public static ImageSource History { get; private set; }
        public static ImageSource Exit { get; private set; }
        public static ImageSource Settings { get; private set; }

        public static ImageSource Infomation { get; private set; }
        public static ImageSource Warning { get; private set; }
        public static ImageSource Error { get; private set; }
        public static ImageSource Success { get; private set; }

        public static void Load()
        {
            App = new BitmapImage(new Uri("pack://application:,,,/Assets/app.png"));
            App.Freeze();
            Plugin = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/plugin.png"));
            Plugin.Freeze();
            AllPlugins = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/allplugins.png"));
            AllPlugins.Freeze();
            Exit = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/exit.png"));
            Exit.Freeze();
            History = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/history.png"));
            History.Freeze();
            Settings = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/settings.png"));
            Settings.Freeze();
            
            Infomation = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/info.png"));
            Warning = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/warning.png"));
            Error = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/error.png"));
            Success = new BitmapImage(new Uri("pack://application:,,,/Assets/Images/success.png"));
            Infomation.Freeze();
            Warning.Freeze();
            Error.Freeze();
            Success.Freeze();
        }
    }
}
