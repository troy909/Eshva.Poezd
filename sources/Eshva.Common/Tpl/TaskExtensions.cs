#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Eshva.Common.Tpl
{
  public static class TaskExtensions
  {
    /// <summary>
    /// A workaround for getting all of AggregateException.InnerExceptions with try/await/catch.
    /// </summary>
    /// <remarks>
    /// Taken from here: https://stackoverflow.com/a/62607500/156991
    /// </remarks>
    public static Task WithAggregatedExceptions(this Task task)
    {
      return task.ContinueWith(
        continuation =>
          continuation.IsFaulted &&
          continuation.Exception is AggregateException exception &&
          (exception.InnerExceptions.Count > 1 || exception.InnerException is AggregateException)
            ? Task.FromException(exception.Flatten())
            : continuation,
        CancellationToken.None,
        TaskContinuationOptions.ExecuteSynchronously,
        TaskScheduler.Default).Unwrap();
    }
  }
}
