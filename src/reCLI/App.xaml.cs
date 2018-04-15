using reCLI.Helper;
using reCLI.Infrastructure;
using reCLI.Infrastructure.Plugins;
using reCLI.Resources;
using reCLI.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace reCLI
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : IDisposable, ISingleInstanceApp
    {
#if DEBUG
        private const string Unique = "reCLI_Unique_DEBUG_Application_Mutex";
#else
        private const string Unique = "reCLI_Unique_Application_Mutex";
#endif

        internal new static App Current { get; set; }
        internal static PluginManager PluginManager { get; set; }

        public const string reCLIName = "reCLI";
        public const string PluginsName = "Plugins";
        public const string DataName = "Data";

        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();
        public static readonly string ProgramDirectory = Directory.GetParent(Assembly.Location).ToString();
        public static readonly string ExecutablePath = Path.Combine(ProgramDirectory, reCLIName + ".exe");
        public static readonly string DataDirectory = Path.Combine(ProgramDirectory, DataName);
        public static readonly string PluginsDirectory = Path.Combine(DataDirectory, PluginsName);
        public static readonly string Version = FileVersionInfo.GetVersionInfo(Assembly.Location).ProductVersion;

        static Plugins.CorePlugin CorePlugin;

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                using (var application = new App())
                {
                    Current = application;
                    application.InitializeComponent();
                    application.Run();
                }
            }
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            RegisterUnhandledExceptionEvent();
            InitializeNotifyIcon();
            Images.Load();
            PluginManager = new PluginManager { PluginsRoot = PluginsDirectory };

            CorePlugin = new Plugins.CorePlugin();
            PluginManager.RegisterPlugin(CorePlugin, Plugins.CorePlugin.Metadata);
            PluginManager.RegisterPlugin(new Plugins.PluginIndicator(), Plugins.PluginIndicator.Metadata);

            await PluginManager.LoadPlugins();

            new MainViewModel();

            var window = new MainWindow();
            Current.MainWindow = window;
            Current.MainWindow.Title = reCLIName;

            MainViewModel.Current.ShowProgressBar();
            await PluginManager.InitializePlugins(x=>new PublicAPIInstance(x.ID));
            MainViewModel.Current.HideProgressBar();

            RegisterExitEvents();

            AutoStartup();
#if DEBUG
            MainViewModel.Current.MainWindowVisibility = Visibility.Visible;
#else
            MainViewModel.Current.MainWindowVisibility = Visibility.Hidden;
#endif
        }

        #region AutoStartup
        private const string StartupPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        public static bool StartupSet()
        {
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(StartupPath, true))
            {
                if (key?.GetValue(reCLIName) is string path)
                {
                    return path == ExecutablePath;
                }
                else
                {
                    return false;
                }
            }
        }
        public static void SetStartup()
        {
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(StartupPath, true))
            {
                key?.SetValue(reCLIName, ExecutablePath);
            }
        }

        private void RemoveStartup()
        {
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(StartupPath, true))
            {
                key?.DeleteValue(reCLIName, false);
            }
        }

        private void AutoStartup()
        {
            if (true)
            {
                if (!StartupSet())
                {
                    SetStartup();
                }
            }
        }

#endregion

#region Register Events
        private void RegisterExitEvents()
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Dispose();
            Current.Exit += (s, e) => Dispose();
            Current.SessionEnding += (s, e) => Dispose();
        }

        public void App_UnhandledExceptionHandle(object sender, UnhandledExceptionEventArgs e)
        {
            //handle non-ui thread exceptions
            CorePlugin.Context.API.ShowMessage($"错误：{(e.ExceptionObject as Exception)?.Message}", TimeSpan.FromSeconds(2), Plugin.MessageIcon.Error);
        }

        public void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //prevent application exist, so the user can copy prompted error info
            CorePlugin.Context.API.ShowMessage($"错误：{e.Exception?.Message}", TimeSpan.FromSeconds(2), Plugin.MessageIcon.Error);
            e.Handled = true;
        }

        /// <summary>
        /// let exception throw as normal is better for Debug
        /// </summary>
        [Conditional("RELEASE")]
        private void RegisterUnhandledExceptionEvent()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += App_UnhandledExceptionHandle;
            AppDomain.CurrentDomain.FirstChanceException += (_, e) =>
            {
                //Log.Exception("|App.RegisterAppDomainExceptions|First Chance Exception:", e.Exception);
            };
        }

#endregion

        private System.Windows.Forms.NotifyIcon _notifyIcon;

        private void InitializeNotifyIcon()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Text = App.reCLIName,
                Icon = reCLI.Properties.Resources.app,
                Visible = true
            };
            
            var menu = new System.Windows.Forms.ContextMenuStrip();
            var items = menu.Items;

            _notifyIcon.DoubleClick+= (o, e) => MainViewModel.Current.MainWindowVisibility = Visibility.Visible;

            var open = items.Add("打开");
            open.Click += (o, e) => MainViewModel.Current.MainWindowVisibility= Visibility.Visible;
            var exit = items.Add("退出");
            exit.Click += (o, e) => MainWindow.Close();

            _notifyIcon.ContextMenuStrip = menu;
            _notifyIcon.MouseClick += (o, e) =>
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    if (menu.Visible)
                    {
                        menu.Close();
                    }
                    else
                    {
                        var p = System.Windows.Forms.Cursor.Position;
                        menu.Show(p);
                    }
                }
            };
        }

        public void OnSecondAppStarted()
        {
            Current.MainWindow.Visibility = Visibility.Visible;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual async void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    await PluginManager.UninitializePlugins();
                    _notifyIcon.Visible = false;
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~App() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
