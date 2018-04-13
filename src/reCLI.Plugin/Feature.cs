using reCLI.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace reCLI.Plugin
{
    public interface IFeatures { }

    public interface IGlobalQuery : IFeatures
    {
        Task<IEnumerable<Answer>> GlobalQuery(Query query, CancellationToken cancellationToken);
    }
}
