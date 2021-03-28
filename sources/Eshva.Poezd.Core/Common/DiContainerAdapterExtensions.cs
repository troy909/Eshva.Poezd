#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Common
{
  /// <summary>
  /// DI-container adapter extensions.
  /// </summary>
  public static class DiContainerAdapterExtensions
  {
    /// <summary>
    /// Gets service of <typeparamref name="TResult" /> type from DI-adapter.
    /// </summary>
    /// <typeparam name="TResult">
    /// The result type of the required service.
    /// </typeparam>
    /// <param name="serviceProvider">
    /// The DI-container adapter.
    /// </param>
    /// <param name="serviceType">
    /// The type of service registered in the container.
    /// </param>
    /// <returns>
    /// An instance of the required service as taken from the DI-container.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The type of the required service is not specified.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The service of <paramref name="serviceType" /> is not registered in the DI-container.
    /// </exception>
    [NotNull]
    public static TResult GetService<TResult>(this IDiContainerAdapter serviceProvider, [NotNull] Type serviceType) =>
      (TResult) serviceProvider.GetService(serviceType ?? throw new ArgumentNullException(nameof(serviceProvider)));

    /// <summary>
    /// Gets service of <typeparamref name="TResult" /> type from DI-adapter.
    /// </summary>
    /// <typeparam name="TResult">
    /// The result type of the required service.
    /// </typeparam>
    /// <param name="serviceProvider">
    /// The DI-container adapter.
    /// </param>
    /// <param name="serviceType">
    /// The type of service registered in the container.
    /// </param>
    /// <param name="exceptionToThrowFactory">
    /// The factory used to construct thrown exception in case the required service is not registered in the DI-container.
    /// </param>
    /// <returns>
    /// An instance of the required service as taken from the DI-container.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// On of arguments is not specified.
    /// </exception>
    [NotNull]
    public static TResult GetService<TResult>(
      this IDiContainerAdapter serviceProvider,
      [NotNull] Type serviceType,
      [NotNull] Func<Exception, Exception> exceptionToThrowFactory)
    {
      if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
      if (exceptionToThrowFactory == null) throw new ArgumentNullException(nameof(exceptionToThrowFactory));

      try
      {
        return (TResult) serviceProvider.GetService(serviceType);
      }
      catch (InvalidOperationException exception)
      {
        throw exceptionToThrowFactory(exception);
      }
    }
  }
}
