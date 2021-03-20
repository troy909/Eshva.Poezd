#region Usings

using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using JetBrains.Annotations;
using Venture.Common.TestingTools.Kafka;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.IntegrationTests
{
  /// <summary>
  /// This class should be placed in the tests project. Otherwise you'll get the error: "The following constructor parameters
  /// did not have matching fixture data."
  /// </summary>
  [UsedImplicitly]
  public class KafkaSetupContainerAsyncFixture : IAsyncLifetime
  {
    public KafkaSetupContainerAsyncFixture()
    {
      _kafkaContainer = new KafkaDockerContainerBuilder()
        .WithKafkaConfiguration(KafkaSetupCollection.Containers.Kafka)
        .WithZookeeperConfiguration(KafkaSetupCollection.Containers.Zookeeper)
        .Build();
      KafkaContainerConfiguration = KafkaSetupCollection.Containers.Kafka;
    }

    public KafkaContainerConfiguration KafkaContainerConfiguration { get; }

    public Task InitializeAsync() => _kafkaContainer.StartAsync();

    public async Task DisposeAsync() => await _kafkaContainer.DisposeAsync();

    private readonly IDockerContainer _kafkaContainer;
  }
}
