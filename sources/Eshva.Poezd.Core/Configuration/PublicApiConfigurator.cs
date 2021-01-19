#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class PublicApiConfigurator
  {
    public PublicApiConfigurator([NotNull] PublicApiConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public PublicApiConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(NotWhitespace, nameof(id));

      _configuration.Id = id;
      return this;
    }

    public PublicApiConfigurator AddQueueNamePattern([NotNull] string queueNamePattern)
    {
      if (string.IsNullOrWhiteSpace(queueNamePattern)) throw new ArgumentException(NotWhitespace, nameof(queueNamePattern));

      _configuration.AddQueueNamePatterns(queueNamePattern);
      return this;
    }

    public PublicApiConfigurator WithPipelineConfigurator<TConfigurator>() where TConfigurator : IPipelineConfigurator
    {
      _configuration.PipelineConfiguratorType = typeof(TConfigurator);
      return this;
    }

    private readonly PublicApiConfiguration _configuration;
    private const string NotWhitespace = "Value cannot be null or whitespace.";
  }
}
