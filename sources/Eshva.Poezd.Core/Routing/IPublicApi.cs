#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public interface IPublicApi
  {
    [NotNull]
    string Id { get; }

    [NotNull]
    PublicApiConfiguration Configuration { get; }

    [NotNull]
    IPipeFitter IngressPipeFitter { get; }

    [NotNull]
    IMessageTypesRegistry MessageTypesRegistry { get; }

    [NotNull]
    IEnumerable<string> GetQueueNamePatterns();
  }
}
