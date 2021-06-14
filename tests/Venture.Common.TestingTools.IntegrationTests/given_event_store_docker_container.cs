#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Eshva.Common.Testing;
using Eshva.Common.Tpl;
using FluentAssertions;
using Venture.Common.TestingTools.EventStoreDb;
using Xunit;

#endregion

namespace Venture.Common.TestingTools.IntegrationTests
{
  [Collection(EventStoreSetupCollection.Name)]
  public class given_event_store_docker_container : IDisposable
  {
    public given_event_store_docker_container()
    {
      _dockerClient = new DockerClientConfiguration().CreateClient();
    }

    [Fact]
    public void when_constructed_with_valid_arguments_it_should_produce_instance_with_expected_state()
    {
      var configuration = CreateEventStoreDbContainerConfiguration();
      var sut = new EventStoreDockerContainer(configuration);

      sut.Configuration.Should().Be(configuration, "constructor should assign container configuration");
    }

    [Fact]
    public async Task when_start_and_stop_it_should_start_docker_container_and_then_stop_it()
    {
      var timeout = Cancellation.TimeoutToken(TimeSpan.FromSeconds(value: 5));

      var containers = await GetRunningContainers();
      containers.Should().NotContain(
        container => container.Names.Any(name => name.Contains(_eventStoreDbContainerName)),
        "container shouldn't be started yet");

      var configuration = CreateEventStoreDbContainerConfiguration();
      var sut = new EventStoreDockerContainer(configuration);

      await sut.StartAsync(timeout);

      containers = await GetRunningContainers();
      containers.Should().Contain(
        container => container.Names.Any(name => name.Contains(_eventStoreDbContainerName)),
        "container should be started");

      await sut.StopAsync(timeout);

      containers = await GetRunningContainers();
      containers.Should().NotContain(
        container => container.Names.Any(name => name.Contains(_eventStoreDbContainerName)),
        "container should be stopped");
    }

    public void Dispose() => _dockerClient.Dispose();

    private Task<IList<ContainerListResponse>> GetRunningContainers() =>
      _dockerClient.Containers.ListContainersAsync(new ContainersListParameters());

    private EventStoreDockerContainerConfiguration CreateEventStoreDbContainerConfiguration() =>
      new EventStoreDockerContainerConfiguration
      {
        ContainerName = _eventStoreDbContainerName,
        ExposedHttpPort = _exposedHttpPort
      };

    private readonly DockerClient _dockerClient;
    private readonly string _eventStoreDbContainerName = $"test-eventstore-{Randomize.String(length: 10)}";
    private readonly ushort _exposedHttpPort = NetworkTools.GetFreeTcpPort(rangeStart: 51000);
  }
}
