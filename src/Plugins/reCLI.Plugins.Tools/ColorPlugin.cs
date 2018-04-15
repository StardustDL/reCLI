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
    public class ColorPlugin : IPluginWithIcon,IGlobalQuery
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        private const int IMG_SIZE = 32;

        PluginContext context;

        ImageSource icon;

        public Guid ID { get => context.ID; }

        public string Name { get => "Color"; }

        public string Description { get => "显示颜色（使用#000000）"; }

        public ImageSource Icon => icon;

        public string Keyword => null;

        Dictionary<string, Answer> colors;

        public Task<IEnumerable<Answer>> GlobalQuery(Query query)
        {
            IEnumerable<Answer> iter()
            {
                if (query.Keyword.StartsWith("#") && (query.Keyword.Length == 4 || query.Keyword.Length == 7))
                {
                    if (!colors.TryGetValue(query.Keyword, out var res))
                    {
                        res = new AnswerWithIcon
                        {
                            Title = $"颜色：{query.Keyword}",
                            Icon = CreateColor(query.Keyword),
                            Execute = _ =>
                            {
                                Clipboard.SetText(query.Keyword);
                                return null;
                            }
                        };
                        colors.Add(query.Keyword, res);
                    }
                    yield return res;
                }
            }

            return Task.Run(() => iter());
        }

        ImageSource CreateColor(string name)
        {
            using (var bitmap = new Bitmap(IMG_SIZE, IMG_SIZE))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                var color = ColorTranslator.FromHtml(name);
                graphics.Clear(color);
                IntPtr hBitmap = bitmap.GetHbitmap();
                ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                if (!DeleteObject(hBitmap))
                {
                    throw new System.ComponentModel.Win32Exception("image transform failed");
                }
                wpfBitmap.Freeze();
                return wpfBitmap;
            }
        }

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                colors = new Dictionary<string, Answer>();
                icon = new BitmapImage(new Uri(Path.Combine(context.PluginDirectory, "Images/color.png"))); ;
                icon.Freeze();
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
