using reCLI.Core;
using reCLI.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reCLI.Plugins.Program.Library
{
    public interface IProgram
    {
        Answer Result(string query);
    }
}
