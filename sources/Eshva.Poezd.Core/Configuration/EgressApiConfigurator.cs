#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class EgressApiConfigurator
  {
    public EgressApiConfigurator(EgressApiConfiguration configuration)
    {
      _configuration = configuration;
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

    public EgressApiConfigurator WithMessageTypesRegistry<TMessageTypesRegistry>()
    {
      _configuration.MessageTypesRegistryType = typeof(TMessageTypesRegistry);
      return this;
    }

    private readonly EgressApiConfiguration _configuration;
  }
}
