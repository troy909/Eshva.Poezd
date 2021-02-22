#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Common
{
  /// <summary>
  /// Poezd adapter for DI-container.
  /// </summary>
  public interface IDiContainerAdapter : IServiceProvider
  {
    /// <summary>
    /// Begins a container scope.
    /// </summary>
    /// <returns>
    /// Disposable object to end gotten scope.
    /// </returns>
    [NotNull]
    IDisposable BeginScope();
  }
}
