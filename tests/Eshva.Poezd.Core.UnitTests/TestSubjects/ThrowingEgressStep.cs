#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class ThrowingEgressStep : IStep<MessagePublishingContext>
  {
    public Task Execute(MessagePublishingContext context) => throw new Exception("Sample exception from a step.");
  }
}
