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

namespace reCLI.Plugins.Tools
{
    public class RandomPlugin : IPlugin
    {
        PluginContext context;

        public Guid ID { get => context.ID; }

        public string Name { get => "Random"; }

        public string Description { get => "生成随机数（使用 rand 1 10）"; }

        public string Keyword => "rand";

        Random rnd = new Random();

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                return true;
            });
        }

        public Task<IEnumerable<Answer>> Query(Query query, CancellationToken cancellationToken)
        {
            IEnumerable<Answer> iter()
            {
                string res = "";
                switch (query.Terms.Length)
                {
                    case 1:
                        res = rnd.NextDouble().ToString();
                        yield return new Answer
                        {
                            Title = res,
                            SubTitle = "随机小数",
                            Priority = 10,
                            Execute = _ =>
                            {
                                Clipboard.SetText(res);
                                return Task.FromResult<Result>(null);
                            }
                        };
                        break;
                    case 2:
                        {
                            if (int.TryParse(query.Terms[1], out int mx))
                            {
                                res = rnd.Next(mx).ToString();
                                yield return new Answer
                                {
                                    Title = res,
                                    SubTitle = $"区间 [{0},{mx}) 随机整数",
                                    Priority = 10,
                                    Execute = _ =>
                                    {
                                        Clipboard.SetText(res);
                                        return Task.FromResult<Result>(null);
                                    }
                                };
                            }
                        }
                        break;
                    case 3:
                        {
                            if (int.TryParse(query.Terms[1], out int mn) && int.TryParse(query.Terms[2], out int mx))
                            {
                                res = rnd.Next(mn,mx).ToString();
                                yield return new Answer
                                {
                                    Title = res,
                                    SubTitle = $"区间 [{mn},{mx}) 随机整数",
                                    Priority = 10,
                                    Execute = _ =>
                                    {
                                        Clipboard.SetText(res);
                                        return Task.FromResult<Result>(null);
                                    }
                                };
                            }
                        }
                        break;
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
