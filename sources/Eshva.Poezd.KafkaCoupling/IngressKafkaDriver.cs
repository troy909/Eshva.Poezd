using Eshva.Poezd.Core.Configuration;

namespace Eshva.Poezd.KafkaCoupling
{
  public class IngressKafkaDriver : IIngressDriver
  {
    public IngressKafkaDriver(IngressKafkaDriverConfiguration configuration)
    {
      _configuration = configuration;
    }

    private readonly IngressKafkaDriverConfiguration _configuration;
  }
}