#region Usings

using System.Collections.Generic;
using DotNet.Testcontainers.Images;
using Venture.Common.TestingTools.Core;

#endregion

namespace Venture.Common.TestingTools.EventStore
{
  public class EventStoreDockerContainerConfiguration : DockerContainerConfiguration
  {
    public ushort ExposedHttpPort { get; set; } = EventStoreHttpPort;

    public override IReadOnlyCollection<KeyValuePair<string, string>> GetVariables() => new Dictionary<string, string>
    {
      {"EVENTSTORE_START_STANDARD_PROJECTIONS", "true"},
      {"EVENTSTORE_INSECURE", "true"},
      {"EVENTSTORE_CLUSTER_SIZE", "1"},
      {"EVENTSTORE_MEM_DB", "true"}
    };

    public override ushort[] GetExposedPorts() => new[] {ExposedHttpPort};

    public override IReadOnlyCollection<IProvidePortBindings.PortBinding> GetPortBindings() =>
      new[] {new IProvidePortBindings.PortBinding(ExposedHttpPort, EventStoreHttpPort)};

    protected override string GetDefaultContainerName() => "test-eventstoredb";

    protected override DockerImage GetDefaultDockerImage() => new DockerImage("eventstore/eventstore:21.2.0-buster-slim");

    public const ushort EventStoreHttpPort = 2113;
  }
}
