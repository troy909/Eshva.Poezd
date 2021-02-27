#region Usings

using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.IntegrationTests.TestSubjects
{
  public class CounterStep : IStep<IPocket>
  {
    public CounterStep(Properties props)
    {
      Props = props;
    }

    public Properties Props { get; }

    public Task Execute(IPocket context)
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
