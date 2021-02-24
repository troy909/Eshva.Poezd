#region Usings

using System.Threading.Tasks;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests.TestSubjects
{
  public class MessageHandler : IHandleMessageOfType<Message02>
  {
    public bool IsExecuted { get; private set; }

    public Task Handle(Message02 message, VentureContext context)
    {
      IsExecuted = true;
      return Task.CompletedTask;
    }
  }
}
