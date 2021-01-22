#region Usings

#endregion

namespace Venture.Common.TestingTools
{
  [Collection(Name)]
  public class KafkaSetupCollection
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
