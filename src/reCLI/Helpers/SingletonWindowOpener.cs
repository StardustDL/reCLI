using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace reCLI.Helpers
{
    class SingletonWindowOpener
    {
        public static T Open<T>(params object[] args) where T : Window
        {
            var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.GetType() == typeof(T))
                         ?? (T)Activator.CreateInstance(typeof(T), args);
            Application.Current.MainWindow.Hide();
            window.Show();
            window.Focus();

            return (T)window;
        }
    }
}
