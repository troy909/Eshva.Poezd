#region Usings

using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class BreakingIngressStep : IStep<IPocket>
  {
    public Task Execute(IPocket context) =>
      throw new BreakThisMessageHandlingException("Here should be some reason why this message is skipped.");
  }
}
