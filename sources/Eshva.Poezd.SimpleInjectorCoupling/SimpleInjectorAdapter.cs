#region Usings

using System;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;
using SimpleInjector;
using SimpleInjector.Lifestyles;

#endregion

namespace Eshva.Poezd.SimpleInjectorCoupling
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
    public object GetService([NotNull] Type serviceType) =>
      _container.GetInstance(serviceType ?? throw new ArgumentNullException(nameof(serviceType)));

    /// <inheritdoc />
    public IDisposable BeginScope() => AsyncScopedLifestyle.BeginScope(_container);

    private readonly Container _container;
  }
}
