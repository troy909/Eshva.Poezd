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
  public class EmptyIngressPublicApi : IIngressPublicApi
  {
    /// <inheritdoc />
    public IngressPublicApiConfiguration Configuration => new();

    /// <inheritdoc />
    public IPipeFitter PipeFitter { get; } = new EmptyPipeFitter();

    /// <inheritdoc />
    public IMessageTypesRegistry MessageTypesRegistry { get; } = new EmptyMessageTypesRegistry();

    /// <inheritdoc />
    public IHandlerRegistry HandlerRegistry { get; } = new EmptyHandlerRegistry();

    /// <inheritdoc />
    public IEnumerable<string> GetQueueNamePatterns() => Enumerable.Empty<string>();
  }
}
