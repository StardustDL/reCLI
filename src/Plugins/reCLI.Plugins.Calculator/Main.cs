using Mages.Core;
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

namespace reCLI.Plugins.Calculator
{
    public class Main : IPluginWithIcon, IGlobalQuery
    {
        private static Regex RegValidExpressChar;
        private static Regex RegBrackets;
        private static Engine MagesEngine;

        PluginContext context;

        ImageSource icon;

        public Guid ID { get => context.ID; }

        public string Name { get => "Calculator"; }

        public string Description { get => "计算器"; }

        public ImageSource Icon => icon;

        public string Keyword => null;

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                icon = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/calculator.png")));
                icon.Freeze();
                MagesEngine = new Engine();
                RegBrackets = new Regex(@"[\(\)\[\]]", RegexOptions.Compiled);
                RegValidExpressChar = new Regex(
                        @"^(" +
                        @"ceil|floor|exp|pi|e|max|min|det|abs|log|ln|sqrt|" +
                        @"sin|cos|tan|arcsin|arccos|arctan|" +
                        @"eigval|eigvec|eig|sum|polar|plot|round|sort|real|zeta|" +
                        @"bin2dec|hex2dec|oct2dec|" +
                        @"==|~=|&&|\|\||" +
                        @"[ei]|[0-9]|[\+\-\*\/\^\., ""]|[\(\)\|\!\[\]]" +
                        @")+$", RegexOptions.Compiled);
                return true;
            });
        }

        public Task<IEnumerable<Answer>> Query(Query query, CancellationToken cancellationToken) => GlobalQuery(query, cancellationToken);

        public Task<IEnumerable<Answer>> GlobalQuery(Query query, CancellationToken cancellationToken)
        {
            IEnumerable<Answer> iter()
            {
                if (!(query.RawText.Length <= 2          // don't affect when user only input "e" or "i" keyword
                || !RegValidExpressChar.IsMatch(query.RawText)
                || !IsBracketComplete(query.RawText)))
                {
                    var result = MagesEngine.Interpret(query.RawText);

                    if (result.ToString() == "NaN") context.API.ShowMessage("结果不是一个数", TimeSpan.FromSeconds(1), MessageIcon.Warning);
                    else if (result is Mages.Core.Function) context.API.ShowMessage("表达式不完整", TimeSpan.FromSeconds(1), MessageIcon.Warning);

                    string ans = result?.ToString();
                    if (!string.IsNullOrEmpty(ans))
                    {
                        yield return new AnswerWithIcon
                        {
                            Title = $"计算结果：{ans}",
                            Icon = icon,
                            Priority = 300,
                            Execute = _ =>
                            {
                                Clipboard.SetText(ans);
                                return Task.FromResult<Result>(null);
                            }
                        };
                    }
                }
            }
            return Task.Run(() => iter(), cancellationToken);
        }

        public Task Uninitialize()
        {
            return Task.CompletedTask;
        }

        private bool IsBracketComplete(string query)
        {
            var matchs = RegBrackets.Matches(query);
            var leftBracketCount = 0;
            foreach (Match match in matchs)
            {
                if (match.Value == "(" || match.Value == "[")
                {
                    leftBracketCount++;
                }
                else
                {
                    leftBracketCount--;
                }
            }

            return leftBracketCount == 0;
        }
    }
}
