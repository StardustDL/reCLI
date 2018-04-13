using reCLI.Core;
using reCLI.Infrastructure;
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
    public class CorePlugin : IPluginWithIcon, IGlobalQuery
    {
        public static PluginMetadata Metadata
        {
            get => new PluginMetadata
            {
                Name = "Core",
                Author = "StardustDL",
                Description = "基础命令",
                Version = "1.0",
                ID = Guid.NewGuid()
            };
        }

        public ImageSource Icon => Images.App;

        public string Keyword => null;

        public Guid ID { get => Context.ID; }

        public string Name { get => "Core"; }

        public string Description { get => "基础命令"; }

        Dictionary<string, Answer> acceptQuery;

        internal PluginContext Context;

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.Context = context;
                acceptQuery = new Dictionary<string, Answer>
                {
                    [":exit"] = new AnswerWithIcon
                    {
                        Title = "退出 reCLI",
                        OriginalQuery=":exit",
                        Icon = Images.App,
                        Execute = _ => { App.Current.MainWindow.Close(); return Task.FromResult<Result>(null); }
                    },
                    [":plugins"] = new AnswerWithIcon
                    {
                        Title = "查看所有插件",
                        Icon = Images.App,
                        OriginalQuery = ":plugins",
                        Execute = _ =>
                        {
                            List<Answer> answers = new List<Answer>();
                            foreach(var p in App.PluginManager)
                            {
                                var ans = new AnswerWithIcon
                                {
                                    Title = $"{p.Item1.Name}",
                                    SubTitle = p.Item1.Description,
                                    OriginalQuery=p.Item1.Keyword + reCLI.Core.Query.TermSeperator
                                };
                                if (p.Item1 is IPluginWithIcon picon)
                                {
                                    ans.Icon = picon.Icon;//It's null when first init,so must be set here
                                }
                                else
                                {
                                    ans.Icon = Images.Plugin;
                                }
                                answers.Add(ans);
                            }
                            context.API.PushAnswers(answers);
                            return Task.FromResult(Result.NotAutoHide);
                        }
                    }
                };
                return true;
            });
        }

        public Task<IEnumerable<Answer>> Query(Query query, CancellationToken cancellationToken) => GlobalQuery(query,cancellationToken);

        public Task<IEnumerable<Answer>> GlobalQuery(Query query, CancellationToken cancellationToken)
        {
            IEnumerable<Answer> _()
            {
                string q = query.GetTrimedText();
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

            return Task.Run(() => _(),cancellationToken);
        }

        public Task Uninitialize()
        {
            return Task.CompletedTask;
        }
    }
}
