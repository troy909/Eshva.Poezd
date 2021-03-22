#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Common
{
  /// <summary>
  /// The contract of an adapter for a DI-container.
  /// </summary>
  public interface IDiContainerAdapter
  {
    /// <summary>
    /// Begins a container scope.
    /// </summary>
    /// <returns>
    /// Disposable object used to end gotten scope.
    /// </returns>
    [NotNull]
    IDisposable BeginScope();

    /// <summary>
    /// Gets an instance of <typeparamref name="TService" /> type from the DI-container.
    /// </summary>
    /// <typeparam name="TService">
    /// The type of an object to get from the DI-container.
    /// </typeparam>
    /// <returns>
    /// A required service object.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Object of the required type is not found.
    /// </exception>
    [NotNull]
    TService GetService<TService>() where TService : class;

    /// <summary>
    /// Gets an object of <paramref name="serviceType" /> type from the DI-container.
    /// </summary>
    /// <param name="serviceType">
    /// The type of an object to get from the DI-container.
    /// </param>
    /// <returns>
    /// A required service object.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Object of the required type is not found.
    /// </exception>
    [NotNull]
    object GetService([NotNull] Type serviceType);
  }
}
