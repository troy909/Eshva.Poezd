#region Usings

using System.Threading.Tasks;
using Eshva.Common.Testing;
using JetBrains.Annotations;
using Venture.Common.TestingTools.EventStore;
using Xunit;

#endregion

namespace Venture.Common.TestingTools.IntegrationTests
{
  [UsedImplicitly]
  public class EventStoreSetupContainerAsyncFixture : IAsyncLifetime
  {
    public EventStoreSetupContainerAsyncFixture()
    {
      ContainerName = $"test-eventstore-{Randomize.String(length: 10)}";
      ExposedHttpPort = NetworkTools.GetFreeTcpPort(rangeStart: 52000);
      _container = new EventStoreDockerContainer(
        new EventStoreDockerContainerConfiguration
        {
          ContainerName = ContainerName,
          ExposedHttpPort = ExposedHttpPort
        });
    }

    public ushort ExposedHttpPort { get; }

    public string ContainerName { get; }

    public Task InitializeAsync() => _container.StartAsync();

    public async Task DisposeAsync() => await _container.DisposeAsync();

    private readonly EventStoreDockerContainer _container;
  }
}
