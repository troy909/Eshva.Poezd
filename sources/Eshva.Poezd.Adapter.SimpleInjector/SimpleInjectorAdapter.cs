#region Usings

using System;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;
using SimpleInjector;
using SimpleInjector.Lifestyles;

#endregion

namespace Eshva.Poezd.Adapter.SimpleInjector
{
  /// <summary>
  /// Adapter to SimpleInjector DI-container.
  /// </summary>
  public sealed class SimpleInjectorAdapter : IDiContainerAdapter
  {
    public SimpleInjectorAdapter([NotNull] Container container)
    {
      _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    /// <inheritdoc />
    public IDisposable BeginScope() => AsyncScopedLifestyle.BeginScope(_container);

    /// <inheritdoc />
    public object GetService(Type serviceType)
    {
      if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
      try
      {
        return _container.GetInstance(serviceType);
      }
      catch (ActivationException exception)
      {
        throw new InvalidOperationException($"Service of type {serviceType.FullName} not found in DI-container.", exception);
      }
    }

    /// <inheritdoc />
    public TService GetService<TService>() where TService : class
    {
      try
      {
        return _container.GetInstance<TService>();
      }
      catch (ActivationException exception)
      {
        throw new InvalidOperationException($"Service of type {typeof(TService).FullName} not found in DI-container.", exception);
      }
    }

    private readonly Container _container;
  }
}
