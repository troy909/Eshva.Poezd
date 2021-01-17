using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;


namespace Eshva.Poezd.Core.Routing
{
  public interface IMessageBrokerAdapter
  {
    IEnumerable<IStep> GetPipelineSteps();
  }
}