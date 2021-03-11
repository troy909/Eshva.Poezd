#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class EgressApiConfigurator
  {
    public EgressApiConfigurator([NotNull] EgressApiConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public EgressApiConfigurator WithId(string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

      _configuration.Id = id;
      return this;
    }

    public EgressApiConfigurator WithPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.PipeFitterType = typeof(TConfigurator);
      return this;
    }

    public EgressApiConfigurator WithMessageTypesRegistry<TMessageTypesRegistry>() where TMessageTypesRegistry : IEgressMessageTypesRegistry
    {
      _configuration.MessageTypesRegistryType = typeof(TMessageTypesRegistry);
      return this;
    }

    private readonly EgressApiConfiguration _configuration;
  }
}
