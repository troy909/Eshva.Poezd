#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using FluentAssertions;
using Venture.Common.TestingTools.EventStoreDb;
using Xunit;

#endregion

namespace Venture.Common.TestingTools.IntegrationTests
{
  public class given_event_store_db_docker_container : IDisposable
  {
    public given_event_store_db_docker_container()
    {
      _dockerClient = new DockerClientConfiguration().CreateClient();
    }

    [Fact]
    public void when_constructed_with_valid_arguments_it_should_produce_instance_with_expected_state()
    {
      var configuration = CreateEventStoreDbContainerConfiguration();
      var sut = new EventStoreDbDockerContainer(configuration);

      sut.Configuration.Should().Be(configuration, "constructor should assign container configuration");
    }

    [Fact]
    public async Task when_start_and_stop_it_should_start_docker_container_and_then_stop_it()
    {
      var configuration = CreateEventStoreDbContainerConfiguration();
      var sut = new EventStoreDbDockerContainer(configuration);

      var containers = await GetRunningContainers();
      containers.Should().NotContain(
        container => container.Names.Any(name => name.Contains(EventStoreDbContainerName)),
        "container shouldn't be started yet");

      await sut.StartAsync();

      containers = await GetRunningContainers();
      containers.Should().Contain(
        container => container.Names.Any(name => name.Contains(EventStoreDbContainerName)),
        "container should be started");

      await sut.StopAsync();

      containers = await GetRunningContainers();
      containers.Should().NotContain(
        container => container.Names.Any(name => name.Contains(EventStoreDbContainerName)),
        "container should be stopped");
    }

    public void Dispose()
    {
      _dockerClient.Dispose();
    }

    private Task<IList<ContainerListResponse>> GetRunningContainers() =>
      _dockerClient.Containers.ListContainersAsync(new ContainersListParameters());

    private static EventStoreDbDockerContainerConfiguration CreateEventStoreDbContainerConfiguration() =>
      new EventStoreDbDockerContainerConfiguration {ContainerName = EventStoreDbContainerName};

    private readonly DockerClient _dockerClient;
    private const string EventStoreDbContainerName = "TestingToolsEventStoreDb";
  }
}
