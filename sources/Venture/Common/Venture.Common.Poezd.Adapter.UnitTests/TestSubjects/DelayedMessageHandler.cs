#region Usings

using System;
using System.Threading.Tasks;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests.TestSubjects
{
  public class DelayedMessageHandler : IHandleMessageOfType<Message02>
  {
    public DelayedMessageHandler(TimeSpan timeout)
    {
      _timeout = timeout;
    }

    public Task Handle(Message02 message, VentureContext context) => Task.Delay(_timeout);

    private readonly TimeSpan _timeout;
  }
}
