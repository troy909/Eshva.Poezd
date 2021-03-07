#region Usings

using System;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public class EgressPublicApi : IEgressPublicApi
  {
    public EgressPublicApi(EgressPublicApiConfiguration configuration, IServiceProvider serviceProvider)
    {
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

      MessageTypesRegistry = GetMessageTypesRegistry(serviceProvider);
      PipeFitter = GetPipeFitter(serviceProvider);
    }

    public EgressPublicApiConfiguration Configuration { get; }

    public IPipeFitter PipeFitter { get; }

    public IEgressMessageTypesRegistry MessageTypesRegistry { get; }

    private IPipeFitter GetPipeFitter(IServiceProvider serviceProvider)
    {
      var pipeFitter = (IPipeFitter) serviceProvider.GetService(
        Configuration.PipeFitterType,
        type => new PoezdOperationException(
          $"Can not get an instance of public API ingress pipe fitter of type '{type.FullName}'." +
          "You should register this type in DI-container."));
      return pipeFitter;
    }

    private IEgressMessageTypesRegistry GetMessageTypesRegistry(IServiceProvider serviceProvider)
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
