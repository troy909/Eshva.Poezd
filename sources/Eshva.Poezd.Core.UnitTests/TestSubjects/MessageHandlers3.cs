using System.Threading.Tasks;

namespace TestSubjects
{
  public class MessageHandler1_3 : IHandler1<Message1>
  {
    public Task Handle(Message1 message) => Task.CompletedTask;
  }

  public class MessageHandler1_4 : IHandler1<Message1>
  {
    public Task Handle(Message1 message) => Task.CompletedTask;
  }
}
