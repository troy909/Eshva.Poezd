using System.Threading.Tasks;

namespace TestSubjects
{
  public class Message1 { }

  public class Message2 { }

  public interface IHandler1<in TMessage> where TMessage : class
  {
    Task Handle(TMessage message);
  }

  public interface IHandler2<in TMessage> where TMessage : class
  {
    Task Handle(TMessage message);
  }

  public interface IAmNotGood0
  {
    Task Handle();
  }

  public interface IAmNotGood2<TArg1, TArg2>
  {
    Task Handle(TArg1 arg1, TArg2 arg2);
  }
}
