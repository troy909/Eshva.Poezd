#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  public class ParallelHandlersExecutionPolicy : IHandlersExecutionPolicy
  {
    public ParallelHandlersExecutionPolicy([NotNull] ILogger<ParallelHandlersExecutionPolicy> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ExecuteHandlers(
      IEnumerable<HandlerDescriptor> handlers,
      object message,
      VentureContext messageHandlingContext)
    {
      var executionTask = Task.WhenAll(
        handlers.Select(handler => 
          ExecuteHandler(
            handler.OnHandle,
            message,
            messageHandlingContext)));

      try
      {
        await executionTask;
      }
      catch (Exception exception)
      {
        // TODO: Compensate handlers that can compensate.
        throw exception;
      }

      // TODO: Commit handlers that can commit.
    }

    private Task ExecuteHandler(
      Func<object, VentureContext, Task> onHandle,
      object message,
      VentureContext messageHandlingContext)
    {
        var handlerNumber = 0; // TODO: Replace handler number with handler type from handler descriptor.
        _logger.LogDebug("Started to execute a message handler #{HandlerNumber}.", handlerNumber);
        var stopwatch = Stopwatch.StartNew();

        try
        {
          return onHandle(message, messageHandlingContext);
        }
        catch (Exception exception)
        {
          _logger.LogError(
            exception,
            "An error occurred during handling message in a message handler #{HandlerNumber}.",
            handlerNumber);
          messageHandlingContext.Abort();
          throw;
        }
        finally
        {
          stopwatch.Stop();
          _logger.LogDebug(
            "Finish to execute a message handler #{HandlerNumber} in {Elapsed} milliseconds.",
            handlerNumber,
            stopwatch.ElapsedMilliseconds);
        }
    }

    private readonly ILogger<ParallelHandlersExecutionPolicy> _logger;
  }
}
