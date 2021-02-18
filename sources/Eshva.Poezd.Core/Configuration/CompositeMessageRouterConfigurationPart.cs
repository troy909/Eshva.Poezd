#region Usings

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public abstract class CompositeMessageRouterConfigurationPart : IMessageRouterConfigurationPart
  {
    public IEnumerable<string> Validate() =>
      ValidateItself().Union(GetChildConfigurations().SelectMany(part => part.Validate()));

    protected abstract IEnumerable<string> ValidateItself();

    protected abstract IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations();
  }
}
