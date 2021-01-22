#region Usings

using System.Collections.Generic;
using DotNet.Testcontainers.Images;

#endregion

namespace Venture.Common.TestingTools
{
  public class KafkaContainerConfiguration : DockerContainerConfiguration
  {
    public KafkaContainerConfiguration()
    {
      AdvertisedHostName = "localhost";
    }

    public ushort ExposedHttpPort { get; set; } = DefaultKafkaHttpPort;

    public ushort InternalHttpPort { get; set; } = DefaultKafkaHttpPort;

    public string ZookeeperAddress { get; set; }

    public string AdvertisedHostName { get; }

    public override IReadOnlyCollection<KeyValuePair<string, string>> GetVariables() => new[]
    {
      new KeyValuePair<string, string>("KAFKA_BROKER_ID", "1"),
      new KeyValuePair<string, string>("KAFKA_LISTENERS", $"PLAINTEXT://{AdvertisedHostName}:{DefaultKafkaHttpPort}"),
      new KeyValuePair<string, string>("KAFKA_ZOOKEEPER_CONNECT", ZookeeperAddress),
      new KeyValuePair<string, string>("ALLOW_PLAINTEXT_LISTENER", "true"),
      new KeyValuePair<string, string>("KAFKA_CFG_AUTO_CREATE_TOPICS_ENABLE", "true")
    };

    public override ushort[] GetExposedPorts() => new[] {ExposedHttpPort};

    public override IReadOnlyCollection<IProvidePortBindings.PortBinding> GetPortBindings() => new[]
    {
      new IProvidePortBindings.PortBinding(ExposedHttpPort, InternalHttpPort)
    };

    protected override string GetDefaultContainerName() => "test-kafka";

    protected override DockerImage GetDefaultDockerImage() => new DockerImage(@"bitnami/kafka:latest");

    private const int DefaultKafkaHttpPort = 9092;
  }
}
