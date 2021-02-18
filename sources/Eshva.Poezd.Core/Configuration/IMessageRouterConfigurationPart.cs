#region Usings

using System.Collections.Generic;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public interface IMessageRouterConfigurationPart
  {
    IEnumerable<string> Validate();
  }
}
