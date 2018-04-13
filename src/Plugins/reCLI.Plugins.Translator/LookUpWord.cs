using Newtonsoft.Json;
using reCLI.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace reCLI.Plugins.Translator
{
    public class LookUpWord
    {
        public class Word
        {
            public string Name { get; set; }

            public string EnglishPronunciation { get; set; }

            public string AmericanPronunciation { get; set; }

            public string[] Meaning { get; set; }
        }

        public static async Task<string> TranslateENtoCN(string origin)
        {
            return await Task.Run(async () =>
            {
                string url = "http://fy.iciba.com/ajax.php?a=fy";
                    //"&f=auto&t=auto&w=My name is li"
                    string html = await HttpClient.POST(url, $"&f=auto&t=auto&w={System.Web.HttpUtility.UrlEncode(origin)}");
                var res = JsonConvert.DeserializeObject<dynamic>(html);
                return (string)res.content["out"];
            });
        }


        public static async Task<Word> LookUp(string word)
        {
            return await Task.Run(async () =>
            {
                string url = String.Format(@"http://dict.youdao.com/search?q={0}", word);
                string content = await HttpClient.GET(url);
                if (content == null) return null;
                MatchCollection tmp, mean, ps;
                try
                {
                    ps = Regex.Matches(content, "\"phonetic\">(?<pro>(.*?))</span>");
                    tmp = Regex.Matches(content, "\"trans-container\">([\\s\\S]*?)</div>", RegexOptions.Multiline);
                    mean = Regex.Matches(tmp[0].Value, "<li>(?<mean>(.*?))</li>");
                }
                catch
                {
                    return null;
                }
                Word res = new Word() { Name = word };
                if (ps.Count == 2)
                {
                    res.EnglishPronunciation = ps[0].Groups["pro"].Value;
                    res.AmericanPronunciation = ps[1].Groups["pro"].Value;
                }
                else if (ps.Count > 0)
                {
                    res.EnglishPronunciation = res.AmericanPronunciation = ps[0].Groups["pro"].Value;
                }
                List<string> meanlist = new List<string>();
                StringBuilder last = new StringBuilder();
                foreach (Match line in mean)
                {
                    string[] words = line.Groups["mean"].Value.Split('.');
                    if (words.Length == 2)
                    {
                        meanlist.Add($"{last.ToString()}{words[0]}.{words[1]}");
                        last.Clear();
                    }
                    else
                    {
                        last.Append($"{words[0]}.");
                    }
                }
                res.Meaning = meanlist.ToArray();
                return res;
            });
        }
    }
}
