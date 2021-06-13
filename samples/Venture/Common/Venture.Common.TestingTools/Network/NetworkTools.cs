#region Usings

using System;
using System.Linq;
using System.Net.NetworkInformation;

#endregion

namespace Venture.Common.TestingTools.Network
{
  public static class NetworkTools
  {
    public static ushort GetFreePort(ushort rangeStart = DynamicTcpPortRangeStart, ushort rangeFinish = DynamicTcpPortRangeFinish)
    {
      var fromPort = Math.Min(rangeStart, rangeFinish);
      var toPort = Math.Max(rangeStart, rangeFinish);

      var properties = IPGlobalProperties.GetIPGlobalProperties();
      var listeners = properties.GetActiveTcpListeners();
      var openPorts = listeners.Select(item => item.Port).ToArray();

      for (var port = fromPort; port <= toPort; port++)
      {
        if (openPorts.All(openPort => openPort != port)) return port;
      }

      throw new ArgumentException($"Unable to find a free port in range from {fromPort} to {toPort}.");
    }

    private const int DynamicTcpPortRangeStart = 49152;
    private const int DynamicTcpPortRangeFinish = 65535;
  }
}
