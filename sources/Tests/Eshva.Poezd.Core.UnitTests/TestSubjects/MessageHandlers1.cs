using System.Threading.Tasks;

namespace TestSubjects
{
  public class MessageHandler1_1 : IHandler1<Message1>
  {
    public Task Handle(Message1 message) => Task.CompletedTask;
  }
}
