#region Usings

using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using Venture.Common.TestingTools.Kafka;
using Xunit;

#endregion

namespace Venture.IntegrationTests
{
  // ReSharper disable once ClassNeverInstantiated.Global
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
