#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  public static class ServiceProviderExtensions
  {
    public static TService GetService<TService>(this IServiceProvider serviceProvider) =>
      (TService) serviceProvider.GetService(typeof(TService));

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
