using reCLI.Core;
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

namespace reCLI.Plugins.Tools
{
    public class Url : IPluginWithIcon,IGlobalQuery
    {
        //based on https://gist.github.com/dperini/729294
        private const string urlPattern = "^" +
            // protocol identifier
            "(?:(?:https?|ftp)://|)" +
            // user:pass authentication
            "(?:\\S+(?::\\S*)?@)?" +
            "(?:" +
            // IP address exclusion
            // private & local networks
            "(?!(?:10|127)(?:\\.\\d{1,3}){3})" +
            "(?!(?:169\\.254|192\\.168)(?:\\.\\d{1,3}){2})" +
            "(?!172\\.(?:1[6-9]|2\\d|3[0-1])(?:\\.\\d{1,3}){2})" +
            // IP address dotted notation octets
            // excludes loopback network 0.0.0.0
            // excludes reserved space >= 224.0.0.0
            // excludes network & broacast addresses
            // (first & last IP address of each class)
            "(?:[1-9]\\d?|1\\d\\d|2[01]\\d|22[0-3])" +
            "(?:\\.(?:1?\\d{1,2}|2[0-4]\\d|25[0-5])){2}" +
            "(?:\\.(?:[1-9]\\d?|1\\d\\d|2[0-4]\\d|25[0-4]))" +
            "|" +
            // host name
            "(?:(?:[a-z\\u00a1-\\uffff0-9]-*)*[a-z\\u00a1-\\uffff0-9]+)" +
            // domain name
            "(?:\\.(?:[a-z\\u00a1-\\uffff0-9]-*)*[a-z\\u00a1-\\uffff0-9]+)*" +
            // TLD identifier
            "(?:\\.(?:[a-z\\u00a1-\\uffff]{2,}))" +
            ")" +
            // port number
            "(?::\\d{2,5})?" +
            // resource path
            "(?:/\\S*)?" +
            "$";
        Regex reg;

        PluginContext context;

        ImageSource icon;

        public Guid ID { get => context.ID; }

        public string Name { get => "Url"; }

        public string Description { get => "访问网络地址"; }

        public ImageSource Icon => icon;

        public string Keyword => null;

        public bool IsURL(string raw)
        {
            if (String.IsNullOrEmpty(raw) || String.IsNullOrWhiteSpace(raw)) return false;
            raw = raw.ToLower();

            if (reg.Match(raw).Value == raw) return true;

            if (raw == "localhost" || raw.StartsWith("localhost:") ||
                raw == "http://localhost" || raw.StartsWith("http://localhost:") ||
                raw == "https://localhost" || raw.StartsWith("https://localhost:")
                )
            {
                return true;
            }

            return false;
        }

        public Task<IEnumerable<Answer>> GlobalQuery(Query query)
        {
            IEnumerable<Answer> iter()
            {
                var raw = query.RawText;

                if (IsURL(raw))
                {
                    yield return new AnswerWithIcon
                    {
                        Title = raw,
                        Icon = icon,
                        Priority = 8,
                        Execute = _ =>
                        {
                            if (!raw.ToLower().StartsWith("http"))
                            {
                                raw = "http://" + raw;
                            }
                            try
                            {
                                System.Diagnostics.Process.Start(raw);
                                return Task.FromResult<Result>(null);
                            }
                            catch
                            {
                                context.API.ShowMessage($"网址打开失败", TimeSpan.FromSeconds(3), MessageIcon.Error);
                                return Task.FromResult(Result.NotAutoHide);
                            }
                        }
                    };
                }
            }

            return Task.Run(() => iter());
        }

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                icon = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/url.png")));
                icon.Freeze();
                reg = new Regex(urlPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                return true;
            });
        }

        public Task<IEnumerable<Answer>> Query(Query query) => GlobalQuery(query);

        public Task Uninitialize()
        {
            return Task.CompletedTask;
        }
    }
}
