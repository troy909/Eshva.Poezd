#region Usings

using Venture.Common.TestingTools.Kafka;
using Xunit;

#endregion

namespace Venture.IntegrationTests
{
  [CollectionDefinition(Name)]
  public class KafkaSetupCollection : ICollectionFixture<KafkaSetupContainerAsyncFixture>
  {
    public static ContainersConfiguration Containers { get; } = new ContainersConfiguration();

    public const string Name = nameof(KafkaSetupCollection);

    public class ContainersConfiguration
    {
      public KafkaContainerConfiguration Kafka { get; } = new KafkaContainerConfiguration();

      public ZookeeperContainerConfiguration Zookeeper { get; } = new ZookeeperContainerConfiguration();
    }
  }
}
