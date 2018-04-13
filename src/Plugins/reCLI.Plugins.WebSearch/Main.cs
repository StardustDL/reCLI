using Newtonsoft.Json;
using reCLI.Core;
using reCLI.Core.Helpers;
using reCLI.Plugin;
using reCLI.Plugin.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace reCLI.Plugins.WebSearch
{
    public class Main : IPluginWithIcon,IGlobalQuery
    {
        PluginContext context;

        ImageSource icon;

        List<SearchSource> _sources;
        Dictionary<string, SearchSource> sources;

        string sourcesPath = null;

        public Guid ID { get => context.ID; }

        public string Name { get => "Web Search"; }

        public string Description { get => "调用网络搜索"; }

        public ImageSource Icon => icon;

        public string Keyword => null;

        Dictionary<string, Answer> acceptQuery;

        public Task<IEnumerable<Answer>> GlobalQuery(Query query, CancellationToken cancellationToken)
        {
            IEnumerable<Answer> iter()
            {
                foreach (var v in acceptQuery)
                {
                    int s = reCLI.Core.StringMatcher.Score(v.Key, query.Keyword);
                    if (s > 0)
                    {
                        v.Value.Priority = s;
                        v.Value.Title = $"搜索 {query.Arguments}";
                        v.Value.Execute = _ =>
                        {
                            if (string.IsNullOrEmpty(query.Arguments))
                            {
                                context.API.ChangeQuery($"{v.Key}{reCLI.Core.Query.TermSeperator}");
                                return Task.FromResult(Result.NotRealCall);
                            }
                            try
                            {
                                System.Diagnostics.Process.Start(String.Format(sources[v.Key].Url, System.Web.HttpUtility.UrlEncode(query.Arguments)));
                                return Task.FromResult<Result>(null);
                            }
                            catch
                            {
                                context.API.ShowMessage($"网址打开失败", TimeSpan.FromSeconds(3), MessageIcon.Error);
                                return Task.FromResult(Result.NotAutoHide);
                            }
                        };
                        yield return v.Value;
                    }
                }
            }
            return Task.Run(() => iter(),cancellationToken);
        }

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                icon = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/web_search.png")));
                icon.Freeze();
                sourcesPath = Path.Combine(context.PluginDirectory, "sources.json");
                _sources = JsonConvert.DeserializeObject<List<SearchSource>>(FileIO.ReadText(sourcesPath));
                sources = new Dictionary<string, SearchSource>();
                acceptQuery = new Dictionary<string, Answer>();
                foreach (var v in _sources)
                {
                    v.Image = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, v.IconPath)));
                    v.Image.Freeze();
                    sources.Add(v.ActionKeyword, v);
                    acceptQuery.Add(v.ActionKeyword, new AnswerWithIcon
                    {
                        SubTitle = v.Title,
                        Icon = v.Image,
                    });
                }

                return true;
            });
        }

        public Task<IEnumerable<Answer>> Query(Query query, CancellationToken cancellationToken) => GlobalQuery(query,cancellationToken);

        public Task Uninitialize()
        {
            return Task.CompletedTask;
        }
    }
}
