#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public interface IPublicApiAdapter
  {
    IEnumerable<IStep> GetPipelineSteps();
  }
}
