#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion


namespace Eshva.Poezd.Core.DependencyInjection
{
  public sealed class DependencyInjector
  {
    /// <summary>
    /// Starts a new resolution context, resolving an instance of the given <typeparamref name="TService"/>.
    /// </summary>
    public ResolutionResult<TService> Get<TService>()
    {
      var resolutionContext = new ResolutionContext(_registrations);
      return new ResolutionResult<TService>(resolutionContext.Get<TService>(), resolutionContext.TrackedInstances);
    }

    /// <summary>
    /// Registers a factory method that can provide an instance of <typeparamref name="TService"/>. Optionally,
    /// the supplied <paramref name="description"/> will be used to report more comprehensible errors in case of
    /// conflicting registrations.
    /// </summary>
    public void Register<TService>(Func<IResolutionContext, TService> resolverMethod, string description = null) =>
      Register(resolverMethod, description : description, isDecorator : false);

    /// <summary>
    /// Registers a decorator factory method that can provide an instance of <typeparamref name="TService"/> 
    /// (i.e. the resolver is expected to call <see cref="IResolutionContext.Get{TService}"/> where TService
    /// is <typeparamref name="TService"/>. Optionally, the supplied <paramref name="description"/> will be used 
    /// to report more comprehensible errors in case of conflicting registrations.
    /// </summary>
    public void Decorate<TService>(Func<IResolutionContext, TService> resolverMethod, string description = null) =>
      Register(resolverMethod, description : description, isDecorator : true);

    /// <summary>
    /// Returns whether there exists a registration for the specified <typeparamref name="TService"/>.
    /// </summary>
    public bool Has<TService>(bool isPrimary = true) => HaveRegistrationFor<TService>(isPrimary, _registrations);

    private static bool HaveRegistrationFor<TService>(bool primary, IReadOnlyDictionary<Type, Registration> registrations)
    {
      var key = typeof(TService);

      if (!registrations.ContainsKey(key))
      {
        return false;
      }

      var registration = registrations[key];

      if (registration.PrimaryServiceFactory != null)
      {
        return true;
      }

      return !primary && registration.Decorators.Any();
    }

    private void Register<TService>(Func<IResolutionContext, TService> resolverMethod, bool isDecorator, string description)
    {
      var typeResolver = GetOrCreateRegistration<TService>();
      var serviceFactory = new ServiceFactory<TService>(resolverMethod, description : description, isDecorator : isDecorator);

      if (!isDecorator &&
          typeResolver.PrimaryServiceFactory != null)
      {
        throw new InvalidOperationException(
          $"Attempted to register {serviceFactory}, but a primary registration already exists: {typeResolver.PrimaryServiceFactory}");
      }

      typeResolver.AddServiceFactory(serviceFactory);
    }

    private Registration GetOrCreateRegistration<TService>()
    {
      if (_registrations.TryGetValue(typeof(TService), out var registration))
      {
        return registration;
      }

      registration = new Registration();
      _registrations[typeof(TService)] = registration;

      return registration;
    }

    private readonly Dictionary<Type, Registration> _registrations = new Dictionary<Type, Registration>();

    private sealed class Registration
    {
      public ServiceFactory PrimaryServiceFactory { get; private set; }

      public List<ServiceFactory> Decorators { get; } = new List<ServiceFactory>();

      public void AddServiceFactory(ServiceFactory serviceFactory)
      {
        if (serviceFactory.IsDecorator)
        {
          Decorators.Insert(0, serviceFactory);
        }
        else
        {
          PrimaryServiceFactory = serviceFactory;
        }
      }
    }

    private abstract class ServiceFactory
    {
      protected ServiceFactory(bool isDecorator)
      {
        IsDecorator = isDecorator;
      }

      public bool IsDecorator { get; }
    }

    private sealed class ServiceFactory<TService> : ServiceFactory
    {
      public ServiceFactory(Func<IResolutionContext, TService> factory, bool isDecorator, string description)
        : base(isDecorator)
      {
        _factory = factory;
        _description = description;
      }

      public TService Create(IResolutionContext context) => _factory(context);

      public override string ToString()
      {
        var role = IsDecorator ? "decorator ->" : "primary ->";
        var type = typeof(TService);

        return !string.IsNullOrWhiteSpace(_description)
          ? $"{role} {type} ({_description})"
          : $"{role} {type}";
      }

      private readonly Func<IResolutionContext, TService> _factory;
      private readonly string _description;
    }

    private sealed class ResolutionContext : IResolutionContext
    {
      public ResolutionContext(Dictionary<Type, Registration> registrations)
      {
        _registrations = registrations;
      }

      public IEnumerable TrackedInstances => _resolvedInstances.ToList();

      public bool Has<TService>(bool isPrimary = true) => HaveRegistrationFor<TService>(isPrimary, _registrations);

      public TService Get<TService>()
      {
        var serviceType = typeof(TService);

        if (_instances.TryGetValue(serviceType, out var existingInstance))
        {
          return (TService)existingInstance;
        }

        if (!_registrations.ContainsKey(serviceType))
        {
          throw new ResolutionException($"Could not find resolver for {serviceType}.");
        }

        if (!_decoratorDepth.ContainsKey(serviceType))
        {
          _decoratorDepth[serviceType] = 0;
        }

        var registration = _registrations[serviceType];
        var depth = _decoratorDepth[serviceType]++;

        try
        {
          var resolver = registration
                         .Decorators
                         .Cast<ServiceFactory<TService>>()
                         .Skip(depth)
                         .FirstOrDefault()
                         ?? (ServiceFactory<TService>)registration.PrimaryServiceFactory;

          var instance = resolver.Create(this);

          _instances[serviceType] = instance;

          if (!_resolvedInstances.Contains(instance))
          {
            _resolvedInstances.Add(instance);
          }

          return instance;
        }
        catch (Exception exception)
        {
          throw new ResolutionException(
            $"Could not resolve {serviceType} with decorator depth {depth} - registrations: {string.Join("; ", registration)}",
            exception);
        }
        finally
        {
          _decoratorDepth[serviceType]--;
        }
      }

      readonly Dictionary<Type, int> _decoratorDepth = new Dictionary<Type, int>();
      readonly Dictionary<Type, Registration> _registrations;
      readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
      readonly List<object> _resolvedInstances = new List<object>();
    }
  }
}
