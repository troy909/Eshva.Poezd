#region Usings

using System.Collections.Generic;
using DotNet.Testcontainers.Images;

#endregion

namespace Venture.Common.TestingTools.Core
{
  public abstract class DockerContainerConfiguration : IProvideEnvironmentVariables, IProvideExposedPorts, IProvidePortBindings
  {
    public string ContainerName
    {
      get => _containerName ?? GetDefaultContainerName();
      set => _containerName = value;
    }

    public DockerImage DockerImage
    {
      get => _dockerImage ?? GetDefaultDockerImage();
      set => _dockerImage = value;
    }

    public abstract IReadOnlyCollection<KeyValuePair<string, string>> GetVariables();

    public abstract ushort[] GetExposedPorts();

    public abstract IReadOnlyCollection<IProvidePortBindings.PortBinding> GetPortBindings();

    protected abstract string GetDefaultContainerName();

    protected abstract DockerImage GetDefaultDockerImage();

    private string _containerName;
    private DockerImage _dockerImage;
  }
}
