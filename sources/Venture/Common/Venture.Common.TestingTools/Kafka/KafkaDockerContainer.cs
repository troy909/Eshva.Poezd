#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using Venture.Common.TestingTools.Core;

#endregion

namespace Venture.Common.TestingTools.Kafka
{
  public class KafkaDockerContainer : DockerContainerAdapter
  {
    public KafkaDockerContainer(KafkaContainerConfiguration kafkaConfiguration, ZookeeperContainerConfiguration zookeeperConfiguration)
    {
      KafkaConfiguration = kafkaConfiguration ?? throw new ArgumentNullException(nameof(kafkaConfiguration));
      ZookeeperConfiguration = zookeeperConfiguration ?? throw new ArgumentNullException(nameof(zookeeperConfiguration));
    }

    public KafkaContainerConfiguration KafkaConfiguration { get; }

    public ZookeeperContainerConfiguration ZookeeperConfiguration { get; }

    protected override IDockerContainer GetMainContainer() => _kafkaContainer;

    protected override async Task StartContainers(CancellationToken ct)
    {
      _zookeeperContainer = BuildZookeeperContainer();
      await _zookeeperContainer.StartAsync(ct);

      KafkaConfiguration.ZookeeperAddress = $"{_zookeeperContainer.IpAddress}:{ZookeeperConfiguration.ExposedHttpPort}";
      _kafkaContainer = BuildKafkaContainer();
      await _kafkaContainer.StartAsync(ct);
    }

    protected override async Task StopContainers(CancellationToken ct)
    {
      await _kafkaContainer.StopAsync(ct);
      await _zookeeperContainer.StopAsync(ct);
    }

    protected override async Task DisposeContainers()
    {
      await _kafkaContainer.DisposeAsync();
      await _zookeeperContainer.DisposeAsync();
    }

    private TestcontainersContainer BuildKafkaContainer() =>
      new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage(KafkaConfiguration.DockerImage)
        .WithName(KafkaConfiguration.ContainerName)
        .WithEnvironmentVariablesFrom(KafkaConfiguration)
        .WithPortBindingsFrom(KafkaConfiguration)
        .WithExposedPortsFrom(KafkaConfiguration)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(KafkaConfiguration.ExposedHttpPort))
        .WithCleanUp(cleanUp: true)
        .Build();

    private TestcontainersContainer BuildZookeeperContainer() =>
      new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage(ZookeeperConfiguration.DockerImage)
        .WithName(ZookeeperConfiguration.ContainerName)
        .WithEnvironmentVariablesFrom(ZookeeperConfiguration)
        .WithPortBindingsFrom(ZookeeperConfiguration)
        .WithExposedPortsFrom(ZookeeperConfiguration)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(ZookeeperConfiguration.ExposedHttpPort))
        .WithCleanUp(cleanUp: true)
        .Build();

    private IDockerContainer _kafkaContainer;
    private IDockerContainer _zookeeperContainer;
  }
}
