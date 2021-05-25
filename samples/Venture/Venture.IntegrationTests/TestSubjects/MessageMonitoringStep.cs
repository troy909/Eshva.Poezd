#region Usings

using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.IntegrationTests.TestSubjects
{
  public class MessageMonitoringStep : IStep<MessageHandlingContext>
  {
    public MessageMonitoringStep(Properties props)
    {
      _props = props;
    }

    public Task Execute(MessageHandlingContext context)
    {
      _props.Counter++;
      _props.LastMessageKey = context.Key;
      _props.LastMessagePayload = context.Payload;
      return Task.CompletedTask;
    }

    private readonly Properties _props;

    public class Properties
    {
      public int Counter { get; set; }

      public object LastMessageKey { get; set; }

      public object LastMessagePayload { get; set; }
    }
  }
}
