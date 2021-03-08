#region Usings

using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.IntegrationTests.TestSubjects
{
  public class FinishTestStep : IStep<MessageHandlingContext>
  {
    public FinishTestStep(Properties props)
    {
      Props = props;
    }

    public Properties Props { get; }

    public Task Execute(MessageHandlingContext context)
    {
      Props.Semaphore.Release();
      return Task.CompletedTask;
    }

    public class Properties
    {
      public Properties(int initialCount = 0, int maxCount = 1)
      {
        Semaphore = new SemaphoreSlim(initialCount, maxCount);
      }

      public SemaphoreSlim Semaphore { get; }
    }
  }
}
