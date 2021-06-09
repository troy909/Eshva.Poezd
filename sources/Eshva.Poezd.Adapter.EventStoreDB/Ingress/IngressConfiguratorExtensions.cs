#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.Ingress
{
  public static class IngressConfiguratorExtensions
  {
    public static BrokerIngressConfigurator WithEventStoreDbDriver(
      this BrokerIngressConfigurator brokerIngress,
      [System.Diagnostics.CodeAnalysis.NotNull] Action<BrokerIngressEventStoreDbDriverConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new BrokerIngressEventStoreDbDriverConfiguration();
      configurator(new BrokerIngressEventStoreDbDriverConfigurator(configuration));
      IBrokerIngressDriverConfigurator driverConfigurator = brokerIngress;
      driverConfigurator.SetDriver(new BrokerIngressEventStoreDbDriver(configuration), configuration);
      return brokerIngress;
    }
  }

  public class BrokerIngressEventStoreDbDriver : IBrokerIngressDriver
  {
    public BrokerIngressEventStoreDbDriver(BrokerIngressEventStoreDbDriverConfiguration configuration) { }

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    public void Initialize(
      IBrokerIngress brokerIngress,
      IEnumerable<IIngressApi> apis,
      IDiContainerAdapter serviceProvider)
    {
      throw new NotImplementedException();
    }

    public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default) =>
      throw new NotImplementedException();
  }

  public class BrokerIngressEventStoreDbDriverConfiguration : IMessageRouterConfigurationPart
  {
    public IEnumerable<string> Validate() => throw new NotImplementedException();
  }

  public class BrokerIngressEventStoreDbDriverConfigurator
  {
    public BrokerIngressEventStoreDbDriverConfigurator(BrokerIngressEventStoreDbDriverConfiguration configuration)
    {
      throw new NotImplementedException();
    }

    public BrokerIngressEventStoreDbDriverConfigurator WithConnection(EventStoreDbConnectionConfiguration connectionConfiguration)
    {
      throw new NotImplementedException();
    }

    public BrokerIngressEventStoreDbDriverConfigurator WithHeaderValueCodec<T>()
    {
      throw new NotImplementedException();
    }
  }

  public class EventStoreDbConnectionConfiguration { }

  /// <summary>
  /// UTF-8 header value codec.
  /// </summary>
  public class Utf8ByteStringHeaderValueCodec : IHeaderValueCodec
  {
    /// <inheritdoc />
    public string Decode(byte[] value) => value != null ? Encoding.UTF8.GetString(value) : string.Empty;

    /// <inheritdoc />
    public byte[] Encode(string value) => value != null ? Encoding.UTF8.GetBytes(value) : new byte[0];
  }

  public interface IHeaderValueCodec
  {
    [Pure]
    [NotNull]
    string Decode([CanBeNull] byte[] value);

    [Pure]
    [NotNull]
    byte[] Encode([CanBeNull] string value);
  }
}
