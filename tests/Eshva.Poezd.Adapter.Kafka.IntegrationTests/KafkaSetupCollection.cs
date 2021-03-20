#region Usings

using Venture.Common.TestingTools.Kafka;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.IntegrationTests
{
  /// <summary>
  /// This class should be placed in the tests project. Otherwise you'll get the error: "The following constructor parameters
  /// did not have matching fixture data."
  /// </summary>
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
