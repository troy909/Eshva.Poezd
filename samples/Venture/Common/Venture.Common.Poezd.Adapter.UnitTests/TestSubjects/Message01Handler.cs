#region Usings

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Venture.Common.Application.Ingress;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests.TestSubjects
{
  public class Message01Handler : IMessageHandler<Message01>
  {
    public Message01Handler(ILogger<Message01Handler> logger)
    {
      _logger = logger;
    }

    public Task Handle(Message01 message, VentureIncomingMessageHandlingContext context)
    {
      _logger.LogDebug($"Handling event {typeof(Message01)}.");
      return Task.CompletedTask;
    }

    private readonly ILogger<Message01Handler> _logger;
  }
}
