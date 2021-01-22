#region Usings

using DotNet.Testcontainers.Containers;

#endregion

namespace Venture.Common.TestingTools.Kafka
{
  public class KafkaDockerContainerBuilder
  {
    public KafkaDockerContainerBuilder WithKafkaConfiguration(KafkaContainerConfiguration kafkaConfiguration)
    {
      _kafkaConfiguration = kafkaConfiguration;
      return this;
    }

    public KafkaDockerContainerBuilder WithZookeeperConfiguration(ZookeeperContainerConfiguration zookeeperConfiguration)
    {
      _zookeeperConfiguration = zookeeperConfiguration;
      return this;
    }

    public IDockerContainer Build() => new KafkaDockerContainer(_kafkaConfiguration, _zookeeperConfiguration);

    private KafkaContainerConfiguration _kafkaConfiguration = new KafkaContainerConfiguration();
    private ZookeeperContainerConfiguration _zookeeperConfiguration = new ZookeeperContainerConfiguration();
  }
}
