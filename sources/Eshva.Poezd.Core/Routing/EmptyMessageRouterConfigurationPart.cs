#region Usings

using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// An empty message router configuration part.
  /// </summary>
  internal sealed class EmptyMessageRouterConfigurationPart : IMessageRouterConfigurationPart
  {
    /// <inheritdoc />
    public IEnumerable<string> Validate() => Enumerable.Empty<string>();
  }
}
