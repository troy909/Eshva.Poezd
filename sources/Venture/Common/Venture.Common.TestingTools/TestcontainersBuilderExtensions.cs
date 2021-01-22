#region Usings

#endregion

#region Usings

using System.Linq;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Containers.Builders;

#endregion

namespace Venture.Common.TestingTools
{
  public static class TestcontainersBuilderExtensions
  {
    public static ITestcontainersBuilder<TDockerContainer> WithEnvironmentVariablesFrom<TDockerContainer>(
      this ITestcontainersBuilder<TDockerContainer> builder,
      IProvideEnvironmentVariables environmentVariablesProvider)
      where TDockerContainer : IDockerContainer =>
      environmentVariablesProvider.GetVariables()
        .Aggregate(builder, (current, variable) => current.WithEnvironment(variable.Key, variable.Value));

    public static ITestcontainersBuilder<TDockerContainer> WithPortBindingsFrom<TDockerContainer>(
      this ITestcontainersBuilder<TDockerContainer> builder,
      IProvidePortBindings portBindingsProvider)
      where TDockerContainer : IDockerContainer =>
      portBindingsProvider.GetPortBindings()
        .Aggregate(builder, (current, portBinding) => current.WithPortBinding(portBinding.HostPort, portBinding.ContainerPort));

    public static ITestcontainersBuilder<TDockerContainer> WithExposedPortsFrom<TDockerContainer>(
      this ITestcontainersBuilder<TDockerContainer> builder,
      IProvideExposedPorts exposedPortsProvider)
      where TDockerContainer : IDockerContainer =>
      exposedPortsProvider.GetExposedPorts()
        .Aggregate(builder, (current, exposedPort) => current.WithPortBinding(exposedPort));
  }
}
