#region Usings

using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /*
  /// <summary>
  /// A stab for public API.
  /// </summary>
  public class EmptyPublicApi : IPublicApi
  {
    /// <inheritdoc />
    public string Id => typeof(EmptyPublicApi).FullName!;

    /// <inheritdoc />
    public PublicApiConfiguration Configuration => new();

    /// <inheritdoc />
    public IPipeFitter IngressPipeFitter { get; } = new EmptyPipeFitter();

    /// <inheritdoc />
    public IPipeFitter EgressPipeFitter { get; } = new EmptyPipeFitter();

    /// <inheritdoc />
    public IMessageTypesRegistry MessageTypesRegistry { get; } = new EmptyMessageTypesRegistry();

    /// <inheritdoc />
    public IHandlerRegistry HandlerRegistry { get; } = new EmptyHandlerRegistry();

    /// <inheritdoc />
    public IEnumerable<string> GetQueueNamePatterns() => Enumerable.Empty<string>();
  }
*/
}
