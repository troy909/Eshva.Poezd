#region Usings

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Venture.Common.Application.Ingress;
using Venture.WorkPlanner.Messages.V1.Events;

#endregion

namespace Venture.CaseOffice.Application.BindTaskToRequirement
{
  /// <summary>
  /// Binds a task created in the Work Planner to the case requirement it planned for.
  /// </summary>
  public class BindTaskToClaim : IMessageHandler<TaskCreated>
  {
    public BindTaskToClaim(ILogger<BindTaskToClaim> logger)
    {
      _logger = logger;
    }

    /// <inheritdoc />
    public Task Handle(TaskCreated message, VentureIncomingMessageHandlingContext context)
    {
      _logger.LogDebug($"Handling event {typeof(TaskCreated)}.");
      return Task.CompletedTask;
    }

    private readonly ILogger<BindTaskToClaim> _logger;
  }
}
