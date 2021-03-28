#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// An empty ingress API.
  /// </summary>
  [ExcludeFromCodeCoverage]
  internal sealed class EmptyIngressApi : IIngressApi
  {
    /// <inheritdoc />
    public string Id { get; } = "An empty ingress API";

    /// <inheritdoc />
    public IngressApiConfiguration Configuration => new();

    /// <inheritdoc />
    public IPipeFitter PipeFitter { get; } = new EmptyPipeFitter();

    /// <inheritdoc />
    public IIngressApiMessageTypesRegistry MessageTypesRegistry { get; } = new EmptyIngressApiMessageTypesRegistry();

    /// <inheritdoc />
    public IHandlerRegistry HandlerRegistry { get; } = new EmptyHandlerRegistry();

    /// <inheritdoc />
    public Type MessageKeyType => typeof(int);

    /// <inheritdoc />
    public Type MessagePayloadType => typeof(byte[]);

    /// <inheritdoc />
    public IEnumerable<string> GetQueueNamePatterns() => Enumerable.Empty<string>();
  }
}
