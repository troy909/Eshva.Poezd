#region Usings

using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class TestBrokerEgressDriverConfiguration : IMessageRouterConfigurationPart
  {
    public string ErrorToReport { get; set; }

    public IEnumerable<string> Validate() => ErrorToReport != null ? new[] {ErrorToReport} : Enumerable.Empty<string>();
  }
}
