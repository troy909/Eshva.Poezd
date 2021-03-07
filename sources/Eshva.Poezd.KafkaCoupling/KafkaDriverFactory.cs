#region Usings

using System;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  /*
  public class KafkaDriverFactory : IMessageBrokerDriverFactory
  {
    public KafkaDriverFactory([NotNull] IServiceProvider serviceProvider, [NotNull] ILogger<KafkaDriver> driverLogger)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _driverLogger = driverLogger ?? throw new ArgumentNullException(nameof(driverLogger));
    }

    public IMessageBrokerDriver Create() => new KafkaDriver(_serviceProvider, _driverLogger);

    private readonly ILogger<KafkaDriver> _driverLogger;
    private readonly IServiceProvider _serviceProvider;
  }
*/
}
