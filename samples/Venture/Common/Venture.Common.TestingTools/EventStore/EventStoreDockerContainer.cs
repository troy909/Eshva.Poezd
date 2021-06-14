#region Usings

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.OutputConsumers;
using DotNet.Testcontainers.Containers.WaitStrategies;
using JetBrains.Annotations;
using Venture.Common.TestingTools.Core;

#endregion

namespace Venture.Common.TestingTools.EventStore
{
  public class EventStoreDockerContainer : DockerContainerAdapter
  {
    public EventStoreDockerContainer([NotNull] EventStoreDockerContainerConfiguration configuration)
    {
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _container = BuildContainer();
    }

    public EventStoreDockerContainerConfiguration Configuration { get; }

    protected override IDockerContainer GetMainContainer() => _container;

    protected override Task StartContainers(CancellationToken ct) => _container.StartAsync(ct);

    protected override Task StopContainers(CancellationToken ct) => _container.StopAsync(ct);

    protected override async Task DisposeContainers() => await _container.DisposeAsync();

    private TestcontainersContainer BuildContainer() =>
      new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage(Configuration.DockerImage)
        .WithName(Configuration.ContainerName)
        .WithEnvironmentVariablesFrom(Configuration)
        .WithPortBindingsFrom(Configuration)
        .WithExposedPortsFrom(Configuration)
        .WithOutputConsumer(_outputConsumer)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(_outputConsumer.Stdout, "IS LEADER... SPARTA!"))
        .WithCleanUp(cleanUp: true)
        .Build();

    private readonly IDockerContainer _container;
    private readonly IOutputConsumer _outputConsumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());
  }
}
