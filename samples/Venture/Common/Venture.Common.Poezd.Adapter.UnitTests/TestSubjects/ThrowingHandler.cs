#region Usings

using System;
using System.Threading.Tasks;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests.TestSubjects
{
  public class ThrowingHandler : IMessageHandler<Message02>
  {
    public Task Handle(Message02 message, VentureIncomingMessageHandlingContext context) => throw new Exception(TestFail);

    private const string TestFail = "test fail";
  }
}
