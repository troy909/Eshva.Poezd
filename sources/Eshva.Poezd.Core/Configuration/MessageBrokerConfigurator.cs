#region Usings

using System;
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

    public MessageBrokerConfigurator AddExternalService([NotNull] Action<ExternalServiceConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var externalServiceConfiguration = new ExternalServiceConfiguration();
      _configuration.AddExternalService(externalServiceConfiguration);
      configurator(new ExternalServiceConfigurator(externalServiceConfiguration));
      return this;
    }

    private readonly MessageBrokerConfiguration _configuration;
  }
}
