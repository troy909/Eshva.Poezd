#region Usings

using System;
using Eshva.Poezd.Core.DependencyInjection;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class StandardConfigurator<TService>
  {
    internal StandardConfigurator(DependencyInjector dependencyInjector, Options options)
    {
      _dependencyInjector = dependencyInjector ?? throw new ArgumentNullException(nameof(dependencyInjector));
      Options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Registers the given factory function as a resolve of the given <typeparamref name="TService"/> service.
    /// </summary>
    public void Register(Func<IResolutionContext, TService> factoryMethod, string description = null)
    {
      if (factoryMethod == null)
      {
        throw new ArgumentNullException(nameof(factoryMethod));
      }

      _dependencyInjector.Register(factoryMethod, description : description);
    }

    /// <summary>
    /// Registers the given factory function as a resolve of the given <typeparamref name="TService"/> service.
    /// </summary>
    public void Decorate(Func<IResolutionContext, TService> factoryMethod, string description = null)
    {
      if (factoryMethod == null)
      {
        throw new ArgumentNullException(nameof(factoryMethod));
      }

      _dependencyInjector.Decorate(factoryMethod, description : description);
    }

    /// <summary>
    /// Gets a typed configurator for another service. Can be useful if e.g. a configuration extension for a <see cref="ITransport"/>
    /// wants to replace the <see cref="ISubscriptionStorage"/> because it is capable of using the transport layer to do pub/sub.
    /// </summary>
    public StandardConfigurator<TOther> OtherService<TOther>() => new StandardConfigurator<TOther>(_dependencyInjector, Options);

    internal Options Options { get; }

    private readonly DependencyInjector _dependencyInjector;
  }

  public class Options
  {
  }
}
