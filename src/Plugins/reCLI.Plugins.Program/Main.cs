using reCLI.Core;
using reCLI.Plugin;
using reCLI.Plugin.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wox.Plugin.Program.Programs;

namespace reCLI.Plugins.Program
{
    public class Main : IPlugin,IGlobalQuery
    {
        internal static PluginContext context;

        static Settings _settings = new Settings();

        private static readonly object IndexLock = new object();
        private static Win32[] _win32s;

        public Guid ID { get => context.ID; }

        public string Name { get => "Program"; }

        public string Description { get => "访问程序"; }

        public string Keyword => null;

        Random rnd = new Random();

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                Main.context = context;
                IndexPrograms();
                return true;
            });
        }

        public static void IndexPrograms()
        {
            Win32[] w = Win32.All(_settings);

            lock (IndexLock)
            {
                _win32s = w;
            }
        }

        public Task<IEnumerable<Answer>> Query(Query query) => GlobalQuery(query);
        
        public Task Uninitialize()
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Answer>> GlobalQuery(Query query)
        {
            IEnumerable<Answer> iter()
            {
                lock (IndexLock)
                {
                    var results1 = _win32s.AsParallel().Select(p => p.Result(query.RawText));
                    var result = results1.Where(r => r.Priority > 0);
                    return result;
                }
            }

            return Task.Run(() => iter());
        }
    }
}
