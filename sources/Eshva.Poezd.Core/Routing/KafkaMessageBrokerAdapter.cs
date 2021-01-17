using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;


namespace Eshva.Poezd.Core.Routing
{
  public class KafkaMessageBrokerAdapter : IMessageBrokerAdapter
  {
    public IEnumerable<IStep> GetPipelineSteps()
    {
      yield break;
    }
  }
}