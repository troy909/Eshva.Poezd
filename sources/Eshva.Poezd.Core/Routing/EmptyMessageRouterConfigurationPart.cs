#region Usings

using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  internal sealed class EmptyMessageRouterConfigurationPart : IMessageRouterConfigurationPart
  {
    public IEnumerable<string> Validate() => Enumerable.Empty<string>();
  }
}
