using reCLI.Core;
using reCLI.Plugin;
using reCLI.Plugin.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace reCLI.Plugins.Note
{
    public class Main : IPluginWithIcon
    {
        List<string> Notes;
        string notesPath;

        PluginContext context;

        ImageSource icon;

        ImageSource edit;

        public Guid ID { get => context.ID; }

        public string Name { get => "Note"; }

        public string Description { get => "快速记录"; }

        public ImageSource Icon => icon;

        public string Keyword => "note";

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                icon = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/note.png"))); ;
                icon.Freeze();
                edit = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/edit.png"))); ;
                edit.Freeze();
                notesPath = Path.Combine(context.PluginDirectory, "notes.txt");
                Notes = new List<string>();
                if (File.Exists(notesPath))
                {
                    Notes.AddRange(File.ReadAllLines(notesPath, Encoding.UTF8));
                }
                return true;
            });
        }

        public Task<IEnumerable<Answer>> Query(Query query, CancellationToken cancellationToken)
        {
            IEnumerable<Answer> iter()
            {
                if (!String.IsNullOrEmpty(query.Arguments))
                {
                    yield return new AnswerWithIcon
                    {
                        Title = $"记下 {query.Arguments}",
                        Icon = edit,
                        Priority=90,
                        Execute = _ =>
                        {
                            Notes.Add(query.Arguments);
                            return Task.FromResult<Result>(null);
                        }
                    };
                }
                for (int i = Notes.Count - 1; i >= 0; i--)
                {
                    string curnote = Notes[i];
                    yield return new Answer
                    {
                        Title = curnote,
                        SubTitle = "删除：Ctrl + Enter",
                        Priority = StringMatcher.Score(curnote, query.Arguments),
                        Execute = _ =>
                        {
                            if (_.SpecialKeyState.CtrlPressed)//delete
                            {
                                Notes.Remove(curnote);
                                context.API.ChangeQuery(query.RawText);
                                return Task.FromResult(Result.NotAutoHide);
                            }
                            else
                            {
                                Clipboard.SetText(curnote);
                                return Task.FromResult<Result>(null);
                            }
                        }
                    };
                }
            }
            return Task.Run(() => iter(), cancellationToken);
        }

        public Task Uninitialize()
        {
            File.WriteAllLines(notesPath, Notes.ToArray(), Encoding.UTF8);
            return Task.CompletedTask;
        }
    }
}
