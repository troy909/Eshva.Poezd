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
  public class BreakingIngressStep : IStep<MessageHandlingContext>
  {
    public BreakingIngressStep([NotNull] ILogger<BreakingIngressStep> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Execute(MessageHandlingContext context)
    {
      _logger.LogInformation(nameof(BreakingIngressStep));
      _logger.LogDebug($"A message from queue with name '{context.QueueName}' from {context.Broker.Id} has been skipped.");
      context.SkipFurtherMessageHandling();
      return Task.CompletedTask;
    }

    private readonly ILogger<BreakingIngressStep> _logger;
  }
}
