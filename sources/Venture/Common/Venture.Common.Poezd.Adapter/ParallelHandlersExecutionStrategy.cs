#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common.Tpl;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter
{
  public class ParallelHandlersExecutionStrategy : IHandlersExecutionStrategy
  {
    public ParallelHandlersExecutionStrategy([NotNull] ILogger<ParallelHandlersExecutionStrategy> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ExecuteHandlers(
      [NotNull] IEnumerable<HandlerDescriptor> handlers,
      [NotNull] object message,
      [NotNull] VentureContext context)
    {
      if (handlers == null) throw new ArgumentNullException(nameof(handlers));
      if (message == null) throw new ArgumentNullException(nameof(message));
      if (context == null) throw new ArgumentNullException(nameof(context));

      await Task.WhenAll(
          handlers.Select(
            handler => ExecuteHandler(
              handler,
              message,
              context)))
        .WithAggregatedExceptions();
    }

    private async Task ExecuteHandler(
      HandlerDescriptor handler,
      object message,
      VentureContext context)
    {
      _logger.LogDebug(
        "Executing a message handler of type {HandlerType}.",
        handler.Type.FullName);

      var stopwatch = Stopwatch.StartNew();
      try
      {
        await handler.OnHandle(message, context);
      }
      catch (Exception exception)
      {
        _logger.LogError(
          exception,
          "An error occurred during a message handler execution of type {HandlerType}.",
          handler.Type.FullName);
        context.Abort();
        throw;
      }
      finally
      {
        stopwatch.Stop();
        _logger.LogDebug(
          "Executed a message handler of type {HandlerType} in {Elapsed} milliseconds.",
          handler.Type.FullName,
          stopwatch.ElapsedMilliseconds);
      }
    }

    private readonly ILogger<ParallelHandlersExecutionStrategy> _logger;
  }
}
