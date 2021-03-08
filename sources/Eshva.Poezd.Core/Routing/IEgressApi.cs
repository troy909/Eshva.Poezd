#region Usings

using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public interface IEgressApi
  {
    /// <summary>
    /// Gets the egress API configuration.
    /// </summary>
    [NotNull]
    EgressApiConfiguration Configuration { get; }

    /// <summary>
    /// Gets the egress pipe fitter.
    /// </summary>
    [NotNull]
    IPipeFitter PipeFitter { get; }

    /// <summary>
    /// Gets the message type registry.
    /// </summary>
    [NotNull]
    IEgressMessageTypesRegistry MessageTypesRegistry { get; }
  }
}
