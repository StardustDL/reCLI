using Newtonsoft.Json;
using reCLI.Core;
using reCLI.Core.Helpers;
using reCLI.Plugin;
using reCLI.Plugin.UI;
using reCLI.Plugins.FileSearch.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace reCLI.Plugins.FileSearch
{
    public class Main : IPluginWithIcon, IGlobalQuery, Plugin.UI.ISettable
    {
        PluginContext context;

        ImageSource icon;

        ImageSource folder;

        static List<string> paths;

        string sourcesPath = null;

        public Guid ID { get => context.ID; }

        public string Name { get => "File Search"; }

        public string Description { get => "文件搜索"; }

        public ImageSource Icon => icon;

        public string Keyword => null;

        public UIElement SettingPage { get; private set; }

        internal static void LoadPaths()
        {
            paths.Clear();
            paths.AddRange(Settings.Current.Paths.Select(x => Environment.ExpandEnvironmentVariables(x)));
        }

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                icon = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/file_search.png")));
                icon.Freeze();
                folder= new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/folder.png")));
                folder.Freeze();
                sourcesPath = Path.Combine(context.PluginDirectory, "settings.json");
                Settings.Current = JsonConvert.DeserializeObject<Settings>(FileIO.ReadText(sourcesPath));
                paths = new List<string>();
                LoadPaths();
                context.API.UIThreadWork(() => SettingPage = new SettingPage());
                invalidChar = Path.GetInvalidFileNameChars();
                return true;
            });
        }

        const int MaxCount = 10;
        char[] invalidChar;

        public Task<IEnumerable<Answer>> Query(Query query) => GlobalQuery(query);

        public Task<IEnumerable<Answer>> GlobalQuery(Query query)
        {
            bool isfilename(string name)
            {
                foreach (var v in invalidChar)
                {
                    if (name.Contains(v)) return false;
                }
                return true;
            }

            IEnumerable<Answer> iter()
            {
                var name = query.GetTrimedText();
                if (isfilename(name))
                {
                    yield return new AnswerWithIcon
                    {
                        Title = $"搜索文件：{name}",
                        Icon = icon,
                        Execute = _ =>
                        {
                            return Task.Run(() =>
                            {
                                context.API.PushAnswers(FindFiles(name));
                                return Result.NotAutoHide;
                            });
                        }
                    };
                }
            }
            return Task.Run(() => iter());
        }

        IEnumerable<Answer> FindFiles(string searchText)
        {
            foreach (var path in paths)
            {
                if (!Directory.Exists(path)) continue;
                int cnt = 0;
                foreach (var s in Directory.EnumerateFiles(path, $"*", SearchOption.AllDirectories))
                {
                    var name = Path.GetFileName(s);
                    int score = StringMatcher.Score(name, searchText);
                    if (score > 0)
                    {
                        cnt++;
                        yield return new AnswerWithIcon
                        {
                            Title = name,
                            SubTitle = s,
                            Priority = score,
                            Icon = CPublic.ToImageSource(System.Drawing.Icon.ExtractAssociatedIcon(s).ToBitmap()),
                            Execute = _ =>
                            {
                                Process.Start(s);
                                return Task.FromResult<Result>(null);
                            }
                        };
                        if (cnt > MaxCount) break;
                    }

                }
                cnt = 0;
                foreach (var s in Directory.EnumerateDirectories(path, $"*", SearchOption.AllDirectories))
                {
                    var name = Path.GetFileName(s);
                    int score = StringMatcher.Score(name, searchText);
                    if (score > 0)
                    {
                        cnt++;
                        yield return new AnswerWithIcon
                        {
                            Title = name,
                            SubTitle = s,
                            Priority = score,
                            Icon = folder,
                            Execute = _ =>
                            {
                                Process.Start(s);
                                return Task.FromResult<Result>(null);
                            }
                        };
                        if (cnt > MaxCount) break;
                    }
                }
            }
        }

        public Task Uninitialize()
        {
            FileIO.WriteText(sourcesPath,JsonConvert.SerializeObject(Settings.Current));
            return Task.CompletedTask;
        }
    }
}
