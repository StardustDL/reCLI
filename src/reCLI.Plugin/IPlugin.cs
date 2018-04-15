using reCLI.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace reCLI.Plugin
{
    public interface IPlugin
    {
        string Name { get; }

        string Description { get; }

        Guid ID { get; }

        string Keyword { get; }

        Task<bool> Initialize(PluginContext context);

        Task Uninitialize();

        Task<IEnumerable<Answer>> Query(Query query);
    }
}
