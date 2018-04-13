using reCLI.Core;
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

namespace reCLI.Plugins.Music
{
    public class Main : IPluginWithIcon
    {
        const string playerCLI = "Programs/MusicPlayerCLI.exe";

        static Music.Library.MusicAPI.NeteaseMusicAPI worker = new Music.Library.MusicAPI.NeteaseMusicAPI();

        static List<Music.Library.MusicAPI.Song> lastSearchSongs = null;

        static Process LastPlayerProcess { get; set; }

        PluginContext context;

        ImageSource icon;

        ImageSource play,pause,stop;

        public Guid ID { get => context.ID; }

        public string Name { get => "Music"; }

        public string Description { get => "音乐"; }

        public ImageSource Icon => icon;

        public string Keyword => "music";
        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                icon = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/music.png")));
                icon.Freeze();
                play = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/play.png")));
                play.Freeze();
                pause = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/pause.png")));
                pause.Freeze();
                stop = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/stop.png")));
                stop.Freeze();
                return true;
            });
        }

        public Task<IEnumerable<Answer>> Query(Query query, CancellationToken cancellationToken)
        {
            if (LastPlayerProcess == null || LastPlayerProcess.HasExited) LastPlayerProcess = Process.Start(new ProcessStartInfo(Path.Combine(context.PluginDirectory, playerCLI)) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardInput = true });
            IEnumerable<Answer> iter()
            {
                switch (query.Terms.Length)
                {
                    case 1:
                        yield return new Answer
                        {
                            Title = "查找",
                            OriginalQuery = "music find "
                        };
                        yield return new AnswerWithIcon
                        {
                            Title = "播放",
                            OriginalQuery = "music play",
                            Icon=play
                        };
                        yield return new AnswerWithIcon
                        {
                            Title = "暂停",
                            OriginalQuery = "music pause",
                            Icon=pause
                        };
                        yield return new AnswerWithIcon
                        {
                            Title = "停止",
                            OriginalQuery = "music stop",
                            Icon=stop
                        };
                        break;
                    case 2:
                        if (lastSearchSongs != null)
                        {
                            switch (query.Terms[1])
                            {
                                case "play":
                                    yield return new AnswerWithIcon
                                    {
                                        Title = "播放",
                                        Icon = play,
                                        Execute = _ =>
                                        {
                                            StartPlaying();
                                            return Task.FromResult<Result>(null);
                                        }
                                    };
                                     break;
                                case "stop":
                                    yield return new AnswerWithIcon
                                    {
                                        Title = "停止",
                                        Icon=stop,
                                        Execute = _ =>
                                        {
                                            StopPlaying();
                                            return Task.FromResult<Result>(null);
                                        }
                                    };
                                    break;
                                case "pause":
                                    yield return new AnswerWithIcon
                                    {
                                        Title = "暂停",
                                        Icon = pause,
                                        Execute = _ =>
                                        {
                                            PausePlaying();
                                            return Task.FromResult<Result>(null);
                                        }
                                    };
                                    break;
                            }
                        }
                        break;
                    default:
                        if (query.Terms[1] == "find")
                        {
                            context.API.Busying();
                            var name = String.Concat(query.Terms.Skip(2));
                            if (!String.IsNullOrEmpty(name))
                            {
                                var musres = worker.Search(name, 10);
                                if (musres.Code == 200)
                                {
                                    lastSearchSongs = musres.Result.Songs;
                                   foreach(var v in lastSearchSongs)
                                    {
                                        v.ShortAr = String.Join("/", v.Ar.Select(x => x.Name));
                                        BitmapImage s=null;
                                        context.API.UIThreadWork(() => s = new BitmapImage(new Uri(v.Al.PicUrl)));
                                        yield return new AnswerWithIcon
                                        {
                                            Title = v.Name,
                                            SubTitle = $"{v.ShortAr} 的专辑《{v.Al.Name}》",
                                            Icon = s,
                                            Execute = _ =>
                                            {
                                                return Task.Run<Result>(() =>
                                                {
                                                    StartPlaying(worker.GetSongsUrl(new long[] { v.Id }).Data[0].Url);
                                                    return (Result)null;
                                                });
                                            }
                                        };
                                    }
                                }
                                else
                                {
                                    context.API.ShowMessage($"查找失败，返回代码：{musres.Code}", TimeSpan.FromSeconds(1), MessageIcon.Error);
                                }
                            }
                            context.API.Unbusying();
                        }
                        break;
                }
            }

            return Task.Run(() => iter(), cancellationToken);
        }

        public Task Uninitialize()
        {
            if (LastPlayerProcess != null && LastPlayerProcess.HasExited == false) LastPlayerProcess.Kill();
            return Task.CompletedTask;
        }

        void StopPlaying()
        {
            LastPlayerProcess.StandardInput.WriteLineAsync("stop");
        }

        void PausePlaying()
        {
            LastPlayerProcess.StandardInput.WriteLineAsync("pause");
        }

        void StartPlaying(string path = null)
        {
            if (path == null) LastPlayerProcess.StandardInput.WriteLineAsync("play");
            else LastPlayerProcess.StandardInput.WriteLineAsync($"play {path}");
        }
    }
}
