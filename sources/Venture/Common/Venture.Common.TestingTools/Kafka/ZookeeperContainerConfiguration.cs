#region Usings

using System.Collections.Generic;
using DotNet.Testcontainers.Images;
using Venture.Common.TestingTools.Core;

#endregion

namespace Venture.Common.TestingTools.Kafka
{
  public class ZookeeperContainerConfiguration : DockerContainerConfiguration
  {
    public ushort ExposedHttpPort { get; set; } = DefaultZookeeperHttpPort;

    public ushort InternalHttpPort { get; set; } = DefaultZookeeperHttpPort;

    public override IReadOnlyCollection<KeyValuePair<string, string>> GetVariables() =>
      new List<KeyValuePair<string, string>>().AsReadOnly();

    public override ushort[] GetExposedPorts() => new[] {ExposedHttpPort};

    public override IReadOnlyCollection<IProvidePortBindings.PortBinding> GetPortBindings() => new[]
    {
      new IProvidePortBindings.PortBinding(ExposedHttpPort, InternalHttpPort)
    };

    protected override string GetDefaultContainerName() => "test-zookeeper";

    protected override DockerImage GetDefaultDockerImage() => new DockerImage(@"bitnami/kafka:latest");

    private const int DefaultZookeeperHttpPort = 2181;
  }
}
