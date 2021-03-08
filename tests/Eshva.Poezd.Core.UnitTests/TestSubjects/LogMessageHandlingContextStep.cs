#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public sealed class LogMessageHandlingContextStep : IStep<MessageHandlingContext>
  {
    public LogMessageHandlingContextStep([NotNull] ILogger<LogMessageHandlingContextStep> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Execute(MessageHandlingContext context)
    {
      _logger.LogInformation(nameof(LogMessageHandlingContextStep));
      _logger.LogDebug($"A message from queue with name '{context.QueueName}' from {context.Broker.Id} has been received.");
      return Task.CompletedTask;
    }

    private readonly ILogger<LogMessageHandlingContextStep> _logger;
  }
}
