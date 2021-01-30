#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public interface IPublicApi
  {
    string Id { get; }

    PublicApiConfiguration Configuration { get; }

    IPipelineConfigurator IngressPipelineConfigurator { get; }

    IEnumerable<string> GetQueueNamePatterns();
  }
}
