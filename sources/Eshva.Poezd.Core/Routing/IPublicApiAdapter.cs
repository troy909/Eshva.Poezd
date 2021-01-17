using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;


namespace Eshva.Poezd.Core.Routing
{
  public interface IPublicApiAdapter
  {
    IEnumerable<IStep> GetPipelineSteps();
  }
}
