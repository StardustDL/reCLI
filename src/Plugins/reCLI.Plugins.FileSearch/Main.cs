using Newtonsoft.Json;
using reCLI.Core;
using reCLI.Core.Helpers;
using reCLI.Plugin;
using reCLI.Plugin.UI;
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
    public class Main : IPluginWithIcon, IGlobalQuery
    {
        PluginContext context;

        ImageSource icon;

        ImageSource folder;

        Settings settings;

        List<string> paths;

        string sourcesPath = null;

        public Guid ID { get => context.ID; }

        public string Name { get => "File Search"; }

        public string Description { get => "文件搜索"; }

        public ImageSource Icon => icon;

        public string Keyword => null;

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
                settings = JsonConvert.DeserializeObject<Settings>(FileIO.ReadText(sourcesPath));
                paths = new List<string>(settings.Paths.Select(x => Environment.ExpandEnvironmentVariables(x)));
                return true;
            });
        }

        const int MaxCount = 10;

        public Task<IEnumerable<Answer>> Query(Query query, CancellationToken cancellationToken) => GlobalQuery(query, cancellationToken);

        public Task<IEnumerable<Answer>> GlobalQuery(Query query, CancellationToken cancellationToken)
        {
            IEnumerable<Answer> iter()
            {
                string searchText = query.GetTrimedText();
                foreach(var path in paths)
                {
                    if (!Directory.Exists(path)) continue;
                    int cnt = 0;
                    foreach (var s in Directory.EnumerateFiles(path,$"*", SearchOption.TopDirectoryOnly))
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
                    foreach (var s in Directory.EnumerateDirectories(path, $"*", SearchOption.TopDirectoryOnly))
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
                                Priority = StringMatcher.Score(name, query.Arguments),
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
            return Task.Run(() => iter(), cancellationToken);
        }

        public Task Uninitialize()
        {
            return Task.CompletedTask;
        }
    }
}
