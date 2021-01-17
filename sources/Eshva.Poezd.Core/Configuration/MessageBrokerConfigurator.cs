#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class MessageBrokerConfigurator
  {
    public MessageBrokerConfigurator([NotNull] MessageBrokerConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public MessageBrokerConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));

      _configuration.Id = id;
      return this;
    }

    public MessageBrokerConfigurator AddPublicApi([NotNull] Action<PublicApiConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var publicApiConfiguration = new PublicApiConfiguration();
      _configuration.AddPublicApi(publicApiConfiguration);
      configurator(new PublicApiConfigurator(publicApiConfiguration));
      return this;
    }

    public MessageBrokerConfigurator WithPipelineConfigurator<TConfigurator>() where TConfigurator : IPipelineConfigurator
    {
      _configuration.PipelineConfiguratorType = typeof(TConfigurator);
      return this;
    }

    private readonly MessageBrokerConfiguration _configuration;
  }
}
