#region Usings

using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// A stab for ingress public API.
  /// </summary>
  internal sealed class EmptyIngressPublicApi : IIngressPublicApi
  {
    /// <inheritdoc />
    public IngressPublicApiConfiguration Configuration => new();

    /// <inheritdoc />
    public IPipeFitter PipeFitter { get; } = new EmptyPipeFitter();

    /// <inheritdoc />
    public IIngressMessageTypesRegistry MessageTypesRegistry { get; } = new EmptyIngressMessageTypesRegistry();

    /// <inheritdoc />
    public IHandlerRegistry HandlerRegistry { get; } = new EmptyHandlerRegistry();

    /// <inheritdoc />
    public IEnumerable<string> GetQueueNamePatterns() => Enumerable.Empty<string>();
  }
}
