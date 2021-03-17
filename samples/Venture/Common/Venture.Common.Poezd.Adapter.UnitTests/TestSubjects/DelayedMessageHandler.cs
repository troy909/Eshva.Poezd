#region Usings

using System;
using System.Threading.Tasks;
using Venture.Common.Application.Ingress;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests.TestSubjects
{
  public class DelayedMessageHandler : IMessageHandler<Message02>
  {
    public DelayedMessageHandler(TimeSpan timeout)
    {
      _timeout = timeout;
    }

    public Task Handle(Message02 message, VentureIncomingMessageHandlingContext context) => Task.Delay(_timeout);

    private readonly TimeSpan _timeout;
  }
}
