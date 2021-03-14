#region Usings

using System;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public interface IEgressApi
  {
    string Id { get; }

    /// <summary>
    /// Gets the egress API configuration.
    /// </summary>
    [NotNull]
    EgressApiConfiguration Configuration { get; }

    Type MessageKeyType => Configuration.MessageKeyType;

    Type MessagePayloadType => Configuration.MessagePayloadType;

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
