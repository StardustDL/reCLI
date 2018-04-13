using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace reCLI.Plugins.WebSearch
{
    public class SearchSource
    {
        public const string DefaultIcon = "Images/web_search.png";

        public string Title { get; set; }

        public string ActionKeyword { get; set; }
        
        public string IconPath { get; set; } = DefaultIcon;

        [JsonIgnore]
        public ImageSource Image { get; set; }

        public string Url { get; set; }

        public bool Enabled { get; set; }
    }
}
