#region Usings

using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using Xunit;

#endregion

namespace Venture.Common.TestingTools.Kafka
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

    public Task InitializeAsync() => null;

    public Task DisposeAsync() => null;

    private IDockerContainer _kafkaContainer;
  }
}
