#region Usings

using System;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public class EgressApi : IEgressApi
  {
    public EgressApi(EgressApiConfiguration configuration, IDiContainerAdapter serviceProvider)
    {
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

      MessageTypesRegistry = GetMessageTypesRegistry(serviceProvider);
      PipeFitter = GetPipeFitter(serviceProvider);
    }

    public string Id => Configuration.Id;

    public EgressApiConfiguration Configuration { get; }

    /// <inheritdoc />
    public Type MessageKeyType => Configuration.MessageKeyType;

    /// <inheritdoc />
    public Type MessagePayloadType => Configuration.MessagePayloadType;

    public IPipeFitter PipeFitter { get; }

    public IEgressMessageTypesRegistry MessageTypesRegistry { get; }

    private IPipeFitter GetPipeFitter(IDiContainerAdapter serviceProvider)
    {
      var pipeFitter = (IPipeFitter) serviceProvider.GetService(
        Configuration.PipeFitterType,
        type => new PoezdOperationException(
          $"Can not get an instance of an egress API pipe fitter of type '{type.FullName}'." +
          "You should register this type in DI-container."));
      return pipeFitter;
    }

    private IEgressMessageTypesRegistry GetMessageTypesRegistry(IDiContainerAdapter serviceProvider)
    {
      var registry = (IEgressMessageTypesRegistry) serviceProvider.GetService(
        Configuration.MessageTypesRegistryType,
        type => new PoezdOperationException(
          $"Can not get an instance of message types registry of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
      return registry;
    }
  }
}
