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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace reCLI.Plugins.Shell
{
    public class Main : IPluginWithIcon
    {
        PluginContext context;

        ImageSource icon;

        List<CustomCommand> _sources;
        Dictionary<string, CustomCommand> sources;

        string sourcesPath = null;

        public Guid ID { get => context.ID; }

        public string Name { get => "Shell"; }

        public string Description { get => "命令行"; }

        public ImageSource Icon => icon;

        public string Keyword => ">";

        Dictionary<string, Answer> acceptQuery;

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                icon = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/shell.png")));
                icon.Freeze();
                sourcesPath = Path.Combine(context.PluginDirectory, "costums.json");
                _sources = JsonConvert.DeserializeObject<List<CustomCommand>>(FileIO.ReadText(sourcesPath));
                sources = new Dictionary<string, CustomCommand>();
                acceptQuery = new Dictionary<string, Answer>();
                foreach (var v in _sources)
                {
                    sources.Add(v.ActionKeyword, v);
                    acceptQuery.Add(v.ActionKeyword, new AnswerWithIcon
                    {
                        Title = v.Title,
                        SubTitle = v.Content,
                        OriginalQuery = $"> {v.ActionKeyword}",
                        Icon = icon,
                        Execute = _ =>
                        {
                            ExecuteCommand(v.Content);
                            return Task.FromResult<Result>(null);
                        }
                    });
                }

                return true;
            });
        }

        private bool ExistInPath(string filename)
        {
            if (File.Exists(filename))
            {
                return true;
            }
            else
            {
                var values = Environment.GetEnvironmentVariable("PATH");
                if (values != null)
                {
                    foreach (var path in values.Split(';'))
                    {
                        var path1 = Path.Combine(path, filename);
                        var path2 = Path.Combine(path, filename + ".exe");
                        if (File.Exists(path1) || File.Exists(path2))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        private void ExecuteCommand(string command, bool runAsAdministrator = false)
        {
            command = command.Trim();
            command = Environment.ExpandEnvironmentVariables(command);

            ProcessStartInfo info;

            var parts = command.Split(new[] { ' ' }, 2);
            if (parts.Length == 2)
            {
                var filename = parts[0];
                if (ExistInPath(filename))
                {
                    var args = parts[1];
                    info = new ProcessStartInfo
                    {
                        FileName = filename,
                        Arguments = args
                    };
                }
                else
                {
                    info = new ProcessStartInfo(command);
                }
            }
            else
            {
                info = new ProcessStartInfo(command);
            }

            info.UseShellExecute = true;
            info.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            info.Verb = runAsAdministrator ? "runas" : "";

            try
            {
                Process.Start(info);
            }
            catch (Exception e)
            {
                context.API.ShowMessage($"命令未找到： {e.Message}", TimeSpan.FromSeconds(5), MessageIcon.Error);
            }
        }

        public Task<IEnumerable<Answer>> Query(Query query)
        {
            IEnumerable<Answer> iter()
            {
                if (!String.IsNullOrEmpty(query.Arguments))
                {
                    yield return new AnswerWithIcon
                    {
                        Title = $"执行命令：{query.Arguments}",
                        Icon = icon,
                        Priority = 90,
                        Execute = _ =>
                        {
                            ExecuteCommand(query.Arguments);
                            return Task.FromResult<Result>(null);
                        }
                    };
                    foreach (var v in acceptQuery)
                    {
                        int s = reCLI.Core.StringMatcher.Score(v.Key, query.Terms[1]);
                        if (s > 0)
                        {
                            v.Value.Priority = s;
                            yield return v.Value;
                        }
                    }
                }
            }
            return Task.Run(() => iter());
        }

        public Task Uninitialize()
        {
            return Task.CompletedTask;
        }
    }
}
