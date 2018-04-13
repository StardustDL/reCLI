using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reCLI.Plugins.Shell
{
    public class CustomCommand
    {
        public string Title { get; set; }

        public string ActionKeyword { get; set; }

        public string Content { get; set; }
    }
}
