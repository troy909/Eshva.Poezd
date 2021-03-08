#region Usings

using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.IntegrationTests.TestSubjects
{
  public class CounterStep : IStep<MessageHandlingContext>
  {
    public CounterStep(Properties props)
    {
      Props = props;
    }

    public Properties Props { get; }

    public Task Execute(MessageHandlingContext context)
    {
      Props.Counter++;
      return Task.CompletedTask;
    }

    public class Properties
    {
      public int Counter { get; set; }
    }
  }
}
