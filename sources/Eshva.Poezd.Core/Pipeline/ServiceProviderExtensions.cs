#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Extensions for <see cref="IServiceProvider" />.
  /// </summary>
  public static class ServiceProviderExtensions
  {
    /// <summary>
    /// Gets service by its service type. If service isn't found returns exception object made with
    /// <paramref name="makeException" />.
    /// </summary>
    /// <param name="serviceProvider">
    /// The service provider.
    /// </param>
    /// <param name="serviceType">
    /// The type of service that should be gotten.
    /// </param>
    /// <param name="makeException">
    /// Functor to make exception object in case the service isn't found.
    /// </param>
    /// <returns>
    /// An instance of required service or exception object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    public static object GetService(
      this IServiceProvider serviceProvider,
      [NotNull] Type serviceType,
      [NotNull] Func<Type, Exception> makeException)
    {
      if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
      if (makeException == null) throw new ArgumentNullException(nameof(makeException));

      return serviceProvider.GetService(serviceType) ?? makeException(serviceType);
    }
  }
}
