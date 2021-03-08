#region Usings

using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class BreakingIngressStep : IStep<MessageHandlingContext>
  {
    public Task Execute(MessageHandlingContext context) =>
      throw new BreakThisMessageHandlingException("Here should be some reason why this message is skipped.");
  }
}
