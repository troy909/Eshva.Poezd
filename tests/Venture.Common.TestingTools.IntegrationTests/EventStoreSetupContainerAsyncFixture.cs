#region Usings

using System.Threading.Tasks;
using JetBrains.Annotations;
using RandomStringCreator;
using Venture.Common.TestingTools.EventStoreDb;
using Venture.Common.TestingTools.Network;
using Xunit;

#endregion

namespace Venture.Common.TestingTools.IntegrationTests
{
  [UsedImplicitly]
  public class EventStoreSetupContainerAsyncFixture : IAsyncLifetime
  {
    public EventStoreSetupContainerAsyncFixture()
    {
      ContainerName = $"test-eventstore-{new StringCreator().Get(length: 10)}";
      ExposedHttpPort = NetworkTools.GetFreePort(rangeStart: 52000);
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
