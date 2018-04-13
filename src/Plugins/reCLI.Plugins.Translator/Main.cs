using reCLI.Core;
using reCLI.Plugin;
using reCLI.Plugin.UI;
using System;
using System.Collections.Generic;
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

namespace reCLI.Plugins.Translator
{
    public class Main : IPluginWithIcon
    {
        
        PluginContext context;

        ImageSource icon;

        public Guid ID { get => context.ID; }

        public string Name { get => "Dictionary"; }

        public string Description { get => "查单词"; }

        public ImageSource Icon => icon;

        public string Keyword => "dic";

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                icon = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/dic.png"))); ;
                icon.Freeze();
                return true;
            });
        }

        public Task<IEnumerable<Answer>> Query(Query query, CancellationToken cancellationToken)
        {
            IEnumerable<Answer> iter()
            {
                IEnumerable<Answer> GetResult(LookUpWord.Word word)
                {
                    string pro = $"美：{word.AmericanPronunciation} 英：{word.EnglishPronunciation}";
                    yield return new Answer
                    {
                        Title = word.Name,
                        SubTitle = pro,
                        Execute = _ => { Clipboard.SetText(pro); return Task.FromResult<Result>(null); }
                    };
                    foreach (var v in word.Meaning)
                        yield return new Answer
                        {
                            Title = v,
                            Execute = _ => { Clipboard.SetText(v); return Task.FromResult<Result>(null); }
                        };
                }

                yield return new AnswerWithIcon
                {
                    Title = $"查单词 {query.Arguments}",
                    Icon = icon,
                    Priority = 20,
                    Execute = async _ =>
                    {
                        string word = query.Arguments;
                        context.API.Busying();
                        var r = await LookUpWord.LookUp(word);
                        context.API.PushAnswers(GetResult(r));
                        context.API.Unbusying();
                        return Result.NotAutoHide;
                    }
                };
                ;
            }
            return Task.Run(() => iter(),cancellationToken);
        }

        public Task Uninitialize()
        {
            return Task.CompletedTask;
        }
    }
}
