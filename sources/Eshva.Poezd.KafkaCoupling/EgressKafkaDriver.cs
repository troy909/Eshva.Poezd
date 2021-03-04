using Eshva.Poezd.Core.Configuration;

namespace Eshva.Poezd.KafkaCoupling
{
  public class EgressKafkaDriver : IEgressDriver
  {
    public EgressKafkaDriver(EgressKafkaDriverConfiguration configuration)
    {
      _configuration = configuration;
    }

    private readonly EgressKafkaDriverConfiguration _configuration;
  }
}