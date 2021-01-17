#region Usings

using System;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class ExternalServiceConfigurator
  {
    private const string NotWhitespace = "Value cannot be null or whitespace.";

    public ExternalServiceConfigurator([NotNull] ExternalServiceConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public ExternalServiceConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(NotWhitespace, nameof(id));

      _configuration.Id = id;
      return this;
    }

    public ExternalServiceConfigurator AddQueueNamePattern([NotNull] string queueNamePattern)
    {
      if (string.IsNullOrWhiteSpace(queueNamePattern)) throw new ArgumentException(NotWhitespace, nameof(queueNamePattern));

      _configuration.AddQueueNamePatterns(queueNamePattern);
      return this;
    }

    public ExternalServiceConfigurator WithAdapter<TAdapter>()
    {
      _configuration.AdapterType = typeof(TAdapter);
      return this;
    }

    private readonly ExternalServiceConfiguration _configuration;
  }
}
