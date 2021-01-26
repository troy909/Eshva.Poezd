#region Usings

using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using Venture.Common.TestingTools.Kafka;
using Xunit;

#endregion

namespace Venture.IntegrationTests
{
  public class KafkaSetupContainerAsyncFixture : IAsyncLifetime
  {
    public KafkaSetupContainerAsyncFixture()
    {
      _kafkaContainer = new KafkaDockerContainerBuilder()
        .WithKafkaConfiguration(KafkaSetupCollection.Containers.Kafka)
        .WithZookeeperConfiguration(KafkaSetupCollection.Containers.Zookeeper)
        .Build();
    }

    // public Task InitializeAsync() => Task.CompletedTask;
    public Task InitializeAsync() => _kafkaContainer.StartAsync();

    // public async Task DisposeAsync() => await Task.CompletedTask;
    public async Task DisposeAsync() => await _kafkaContainer.DisposeAsync();

    private readonly IDockerContainer _kafkaContainer;
  }
}
