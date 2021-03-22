#region Usings

using System;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// An egress API.
  /// </summary>
  internal class EgressApi : IEgressApi
  {
    /// <summary>
    /// Constructs a new instance of egress API.
    /// </summary>
    /// <param name="configuration">
    /// The egress API configuration.
    /// </param>
    /// <param name="serviceProvider">
    /// Service provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// On of arguments is not specified.
    /// </exception>
    public EgressApi(EgressApiConfiguration configuration, IDiContainerAdapter serviceProvider)
    {
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

      MessageTypesRegistry = GetMessageTypesRegistry(serviceProvider);
      PipeFitter = GetPipeFitter(serviceProvider);
    }

    /// <inheritdoc />
    public string Id => Configuration.Id;

    /// <inheritdoc />
    public EgressApiConfiguration Configuration { get; }

    /// <inheritdoc />
    public Type MessageKeyType => Configuration.MessageKeyType;

    /// <inheritdoc />
    public Type MessagePayloadType => Configuration.MessagePayloadType;

    /// <inheritdoc />
    public IPipeFitter PipeFitter { get; }

    /// <inheritdoc />
    public IEgressApiMessageTypesRegistry MessageTypesRegistry { get; }

    private IPipeFitter GetPipeFitter(IDiContainerAdapter serviceProvider) =>
      serviceProvider.GetService<IPipeFitter>(
        Configuration.PipeFitterType,
        exception => new PoezdConfigurationException(
          $"Can not get an instance of an egress API pipe fitter of type '{Configuration.PipeFitterType}'." +
          "You should register this type in DI-container.",
          exception));

    private IEgressApiMessageTypesRegistry GetMessageTypesRegistry(IDiContainerAdapter serviceProvider) =>
      serviceProvider.GetService<IEgressApiMessageTypesRegistry>(
        Configuration.MessageTypesRegistryType,
        exception => new PoezdConfigurationException(
          $"Can not get an instance of message types registry of type '{Configuration.MessageTypesRegistryType}'. " +
          "You should register this type in DI-container.",
          exception));
  }
}
