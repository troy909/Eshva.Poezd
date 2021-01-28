#region Usings

using Eshva.Poezd.Core.Routing;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  public class KafkaDriverFactory : IMessageBrokerDriverFactory
  {
    public KafkaDriverFactory(ILogger<KafkaDriver> driverLogger)
    {
      _driverLogger = driverLogger;
    }

    public IMessageBrokerDriver Create() => new KafkaDriver(_driverLogger);

    private readonly ILogger<KafkaDriver> _driverLogger;
  }
}
