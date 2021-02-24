#region Usings

using System.Collections.Generic;
using DotNet.Testcontainers.Images;
using Venture.Common.TestingTools.Core;

#endregion

namespace Venture.Common.TestingTools.Kafka
{
  public class KafkaContainerConfiguration : DockerContainerConfiguration
  {
    public ushort BootstrapPort { get; set; } = DefaultBootstrapPort;

    public ushort InternalHttpPort { get; set; } = DefaultBootstrapPort;

    public string ZookeeperAddress { get; set; }

    public string BootstrapServers => $"localhost:{BootstrapPort}";

    public override IReadOnlyCollection<KeyValuePair<string, string>> GetVariables() => new[]
    {
      new KeyValuePair<string, string>(
        "KAFKA_LISTENERS",
        $"INTERNAL://:{DefaultInternalCommunicationPort},EXTERNAL://:{BootstrapPort}"),
      new KeyValuePair<string, string>(
        "KAFKA_ADVERTISED_LISTENERS",
        $"INTERNAL://{ContainerName}:{DefaultInternalCommunicationPort},EXTERNAL://localhost:{BootstrapPort}"),
      new KeyValuePair<string, string>("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "INTERNAL:PLAINTEXT,EXTERNAL:PLAINTEXT"),
      new KeyValuePair<string, string>("KAFKA_INTER_BROKER_LISTENER_NAME", "INTERNAL"),
      new KeyValuePair<string, string>("KAFKA_ZOOKEEPER_CONNECT", ZookeeperAddress),
      new KeyValuePair<string, string>("ALLOW_PLAINTEXT_LISTENER", "true"),
      new KeyValuePair<string, string>("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", "1"),
      new KeyValuePair<string, string>("KAFKA_CFG_AUTO_CREATE_TOPICS_ENABLE", "true")
    };

    public override ushort[] GetExposedPorts() => new[] {BootstrapPort};

    public override IReadOnlyCollection<IProvidePortBindings.PortBinding> GetPortBindings() => new[]
    {
      new IProvidePortBindings.PortBinding(BootstrapPort, InternalHttpPort)
    };

    protected override string GetDefaultContainerName() => "test-kafka";

    protected override DockerImage GetDefaultDockerImage() => new DockerImage(@"bitnami/kafka:latest");

    private const int DefaultBootstrapPort = 9092;
    private const int DefaultInternalCommunicationPort = 29092;
  }
}
