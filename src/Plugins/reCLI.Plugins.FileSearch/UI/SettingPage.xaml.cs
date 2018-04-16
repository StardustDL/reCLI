using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace reCLI.Plugins.FileSearch.UI
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : UserControl
    {
        public SettingPage()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Settings.Current.Paths.Clear();
            int lineCount = txtPaths.LineCount;
            for (int line = 0; line < lineCount; line++)
                Settings.Current.Paths.Add(txtPaths.GetLineText(line).Trim());
            Main.LoadPaths();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtPaths.Clear();
            foreach (var v in Settings.Current.Paths) txtPaths.AppendText($"{v}{Environment.NewLine}");
        }
    }
}
