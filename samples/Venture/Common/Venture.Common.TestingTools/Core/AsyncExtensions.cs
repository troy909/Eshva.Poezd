#region Usings

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.TestingTools.Core
{
  public static class AsyncExtensions
  {
    /// <summary>
    /// Allows a cancellation token to be awaited.
    /// </summary>
    /// <remarks>
    /// Taken from: https://medium.com/@cilliemalan/how-to-await-a-cancellation-token-in-c-cbfc88f28fa2
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [UsedImplicitly]
    public static CancellationTokenAwaiter GetAwaiter(this CancellationToken cancellationToken) =>
      new CancellationTokenAwaiter(cancellationToken);

    /// <summary>
    /// The awaiter for cancellation tokens.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly struct CancellationTokenAwaiter : ICriticalNotifyCompletion
    {
      public CancellationTokenAwaiter(CancellationToken cancellationToken)
      {
        _cancellationToken = cancellationToken;
      }

      private readonly CancellationToken _cancellationToken;

      /// <remarks>
      /// This is called by compiler generated methods when the task has completed.
      /// Instead of returning a result, we just throw an exception.
      /// </remarks>
      [UsedImplicitly]
      public object GetResult()
      {
        if (IsCompleted) return new object();
        // if (IsCompleted) throw new OperationCanceledException(); // From original article.
        throw new InvalidOperationException("The cancellation token has not yet been cancelled.");
      }

      /// <remarks>
      /// Called by compiler generated/.NET internals to check if the task has completed.
      /// </remarks>
      [UsedImplicitly]
      public bool IsCompleted => _cancellationToken.IsCancellationRequested;

      // The compiler will generate stuff that hooks in
      // here. We hook those methods directly into the
      // cancellation token.
      public void OnCompleted(Action continuation) => _cancellationToken.Register(continuation);

      public void UnsafeOnCompleted(Action continuation) => _cancellationToken.Register(continuation);
    }
  }
}
