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
      _props = props;
    }

    public Task Execute(MessageHandlingContext context)
    {
      _props.Semaphore.Release();
      return Task.CompletedTask;
    }

    private readonly Properties _props;

    public class Properties
    {
      public Properties(int totalMessageCount = 1)
      {
        Semaphore = new SemaphoreSlim(initialCount: 0, totalMessageCount);
      }

      public SemaphoreSlim Semaphore { get; }
    }
  }
}
