#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using JetBrains.Annotations;
using Venture.Common.TestingTools.Core;

#endregion

namespace Venture.Common.TestingTools.EventStoreDb
{
  public class EventStoreDbDockerContainer : DockerContainerAdapter
  {
    public EventStoreDbDockerContainer([NotNull] EventStoreDbDockerContainerConfiguration configuration)
    {
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public EventStoreDbDockerContainerConfiguration Configuration { get; }

    protected override IDockerContainer GetMainContainer() => _container;

    protected override Task StartContainers(CancellationToken ct)
    {
      _container = BuildContainer();
      return _container.StartAsync(ct);
    }

    protected override Task StopContainers(CancellationToken ct) => _container.StopAsync(ct);

    protected override async Task DisposeContainers() => await _container.DisposeAsync();

    private TestcontainersContainer BuildContainer() =>
      new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage(Configuration.DockerImage)
        .WithName(Configuration.ContainerName)
        .WithEnvironmentVariablesFrom(Configuration)
        .WithPortBindingsFrom(Configuration)
        .WithExposedPortsFrom(Configuration)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(Configuration.ExposedHttpPort))
        .WithCleanUp(cleanUp: true)
        .Build();

    private IDockerContainer _container;
  }
}
