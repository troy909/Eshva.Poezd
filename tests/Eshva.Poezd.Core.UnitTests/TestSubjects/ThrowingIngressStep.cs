#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class ThrowingIngressStep : IStep<MessageHandlingContext>
  {
    public Task Execute(MessageHandlingContext context) => throw new Exception("Sample exception from a message handler.");
  }
}
