#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Configuration;

#endregion


namespace Eshva.Poezd.RabbitMqCoupling
{
  public static class RabbitMqConfigurationExtensions
  {
    public static string ConnectionString { get; private set; }

    public static MessageBrokerConfigurator UseRabbitMq(this MessageBrokerConfigurator broker, [NotNull] string connectionString)
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
