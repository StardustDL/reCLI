using reCLI.Core;
using reCLI.Plugin;
using reCLI.Plugin.UI;
using reCLI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace reCLI.Plugins
{
    class PluginIndicator : IPluginWithIcon, IGlobalQuery
    {
        public static PluginMetadata Metadata
        {
            get => new PluginMetadata
            {
                Name = "Plugin Indicator",
                Author = "StardustDL",
                Description = "命令提示",
                Version = "1.0",
                ID = Guid.NewGuid()
            };
        }

        public Guid ID { get => context.ID; }

        public string Name { get => "Plugin Indicator"; }

        public string Description { get => "命令提示"; }

        public ImageSource Icon => Images.App;

        public string Keyword => null;

        Dictionary<string, Answer> acceptQuery;

        PluginContext context;

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                return true;
            });
        }

        public Task<IEnumerable<Answer>> Query(Query query, CancellationToken cancellationToken) => GlobalQuery(query, cancellationToken);

        public Task<IEnumerable<Answer>> GlobalQuery(Query query, CancellationToken cancellationToken)
        {
            IEnumerable<Answer> iter()
            {
                if (acceptQuery == null)
                {
                    acceptQuery = new Dictionary<string, Answer>();
                    foreach (var p in App.PluginManager.Where(x => !String.IsNullOrEmpty(x.Item1.Keyword)))
                    {
                        var ans = new AnswerWithIcon
                        {
                            Title = $"插件：{p.Item1.Name}",
                            OriginalQuery=p.Item1.Keyword + reCLI.Core.Query.TermSeperator,
                            SubTitle = p.Item1.Description
                        };
                        if (p.Item1 is IPluginWithIcon picon)
                        {
                            ans.Icon = picon.Icon;//It's null when first init,so must be set here
                        }
                        else
                        {
                            ans.Icon = Images.Plugin;
                        }
                        acceptQuery.Add(p.Item1.Keyword, ans);
                    }
                }
                string q = query.GetTrimedText();
                if (!query.RawText.EndsWith(" "))
                {
                    foreach (var v in acceptQuery)
                    {
                        int s = reCLI.Core.StringMatcher.Score(v.Key, q);
                        if (s > 0)
                        {
                            v.Value.Priority = s;
                            yield return v.Value;
                        }
                    }
                }
            }

            return Task.Run(() => iter(),cancellationToken);
        }

        public Task Uninitialize()
        {
            return Task.CompletedTask;
        }
    }
}
