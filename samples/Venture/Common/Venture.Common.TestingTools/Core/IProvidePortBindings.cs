#region Usings

using System.Collections.Generic;

#endregion

namespace Venture.Common.TestingTools.Core
{
  public interface IProvidePortBindings
  {
    IReadOnlyCollection<PortBinding> GetPortBindings();

    public readonly struct PortBinding
    {
      public PortBinding(ushort hostPort, ushort containerPort)
      {
        HostPort = hostPort;
        ContainerPort = containerPort;
      }

      public ushort HostPort { get; }

      public ushort ContainerPort { get; }
    }
  }
}
