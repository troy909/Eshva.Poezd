#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public sealed class ExecutionEventSubscriptions : IExecutionEventSubscriptions, IRaiseExecutionEvents
  {
    public void Committed([NotNull] Func<IMessageHandlingContext, Task> commitAction)
    {
      if (commitAction == null) throw new ArgumentNullException(nameof(commitAction));

      _committedEventHandlers.Add(commitAction);
    }

    public void Completed([NotNull] Func<IMessageHandlingContext, Task> completedAction)
    {
      if (completedAction == null) throw new ArgumentNullException(nameof(completedAction));

      _completedEventHandlers.Add(completedAction);
    }

    public void Aborted([NotNull] Action<IMessageHandlingContext> abortedAction)
    {
      if (abortedAction == null) throw new ArgumentNullException(nameof(abortedAction));

      _abortedEventHandlers.Add(abortedAction);
    }

    public void Disposed([NotNull] Action<IMessageHandlingContext> disposedAction)
    {
      if (disposedAction == null) throw new ArgumentNullException(nameof(disposedAction));

      _disposedEventHandlers.Add(disposedAction);
    }

    public async Task RaiseCommittedEvent([NotNull] IMessageHandlingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      foreach (var eventHandler in _committedEventHandlers)
      {
        await eventHandler(context);
      }
    }

    public async Task RaiseCompletedEvent([NotNull] IMessageHandlingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      foreach (var eventHandler in _completedEventHandlers)
      {
        await eventHandler(context);
      }
    }

    public void RaiseAbortedEvent([NotNull] IMessageHandlingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      foreach (var eventHandler in _abortedEventHandlers)
      {
        eventHandler(context);
      }
    }

    public void RaiseDisposedEvent([NotNull] IMessageHandlingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      foreach (var eventHandler in _disposedEventHandlers)
      {
        eventHandler(context);
      }
    }

    private readonly IList<Func<IMessageHandlingContext, Task>> _committedEventHandlers = new List<Func<IMessageHandlingContext, Task>>();
    private readonly IList<Func<IMessageHandlingContext, Task>> _completedEventHandlers = new List<Func<IMessageHandlingContext, Task>>();
    private readonly IList<Action<IMessageHandlingContext>> _abortedEventHandlers = new List<Action<IMessageHandlingContext>>();
    private readonly IList<Action<IMessageHandlingContext>> _disposedEventHandlers = new List<Action<IMessageHandlingContext>>();
  }
}
