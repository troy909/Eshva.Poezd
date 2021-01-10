#region Usings

using System;
using Eshva.Poezd.Core.Configuration;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.KafkaCoupling
{
  public static class KafkaConfigurationExtensions
  {
    public static string ConnectionString { get; private set; }

    public static MessageBrokerConfigurator UseKafka(this MessageBrokerConfigurator broker, [NotNull] string connectionString)
    {
      if (string.IsNullOrWhiteSpace(connectionString))
      {
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionString));
      }

      ConnectionString = connectionString;
      return broker;
    }
  }
}
