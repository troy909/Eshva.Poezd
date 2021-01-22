#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;

#endregion

namespace Venture.Common.TestingTools.Core
{
  public abstract class DockerContainerAdapter : IDockerContainer
  {
    public virtual string Id => GetMainContainer().Id;

    public virtual string Name => GetMainContainer().Name;

    public virtual string IpAddress => GetMainContainer().IpAddress;

    public virtual string MacAddress => GetMainContainer().MacAddress;

    public virtual string Hostname => GetMainContainer().Hostname;

    public virtual async ValueTask DisposeAsync()
    {
      await DisposeContainers();
      _isStarted = false;
    }

    public virtual ushort GetMappedPublicPort(int privatePort)
    {
      EnsureContainerIsStarted();
      return GetMainContainer().GetMappedPublicPort(privatePort);
    }

    public virtual ushort GetMappedPublicPort(string privatePort)
    {
      EnsureContainerIsStarted();
      return GetMainContainer().GetMappedPublicPort(privatePort);
    }

    public virtual Task<long> GetExitCode(CancellationToken ct = new CancellationToken())
    {
      EnsureContainerIsStarted();
      return GetMainContainer().GetExitCode(ct);
    }

    public async Task StartAsync(CancellationToken ct = new CancellationToken())
    {
      EnsureContainerIsStopped();
      await StartContainers(ct);
      _isStarted = true;
    }

    public async Task StopAsync(CancellationToken ct = new CancellationToken())
    {
      EnsureContainerIsStarted();
      await StopContainers(ct);
      _isStarted = false;
    }

    public Task<long> ExecAsync(IList<string> command, CancellationToken ct = new CancellationToken())
    {
      EnsureContainerIsStarted();
      return GetMainContainer().ExecAsync(command, ct);
    }

    protected abstract IDockerContainer GetMainContainer();

    protected abstract Task StartContainers(CancellationToken ct);

    protected abstract Task StopContainers(CancellationToken ct);

    protected abstract Task DisposeContainers();

    private void EnsureContainerIsStarted()
    {
      if (!_isStarted) throw new InvalidOperationException("Docker-container isn't ran yet.");
    }

    private void EnsureContainerIsStopped()
    {
      if (_isStarted) throw new InvalidOperationException("Docker-container isn't ran yet.");
    }

    private bool _isStarted;
  }
}
