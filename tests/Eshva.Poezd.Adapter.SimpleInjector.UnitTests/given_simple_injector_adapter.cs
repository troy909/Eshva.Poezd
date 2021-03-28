#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.SimpleInjector.UnitTests
{
  public class given_simple_injector_adapter
  {
    [Fact]
    public void when_get_registered_service_with_concrete_method_it_should_return_instance()
    {
      var sut = new SimpleInjectorAdapter(MakeContainer());
      sut.GetService(typeof(IService1)).Should().BeOfType<Service1>();
      sut.GetService(typeof(Service1)).Should().BeOfType<Service1>();
      sut.GetService(typeof(Service2)).Should().BeOfType<Service2>();
    }

    [Fact]
    public void when_get_unregistered_service_with_concrete_method_it_should_fail()
    {
      var adapter = new SimpleInjectorAdapter(MakeContainer());
      Action sut = () => adapter.GetService(typeof(UnregisteredService));
      sut.Should().ThrowExactly<InvalidOperationException>().Where(exception => exception.Message.Contains(nameof(UnregisteredService)));
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_get_unspecified_service_it_should_fail()
    {
      var adapter = new SimpleInjectorAdapter(MakeContainer());
      Action sut = () => adapter.GetService(serviceType: null);
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("serviceType"));
    }

    [Fact]
    public void when_get_registered_service_with_generic_method_it_should_return_instance()
    {
      var sut = new SimpleInjectorAdapter(MakeContainer());
      sut.GetService<IService1>().Should().BeOfType<Service1>();
      sut.GetService<Service1>().Should().BeOfType<Service1>();
      sut.GetService<Service2>().Should().BeOfType<Service2>();
    }

    [Fact]
    public void when_get_unregistered_service_with_generic_method_it_should_fail()
    {
      var adapter = new SimpleInjectorAdapter(MakeContainer());
      Action sut = () => adapter.GetService<UnregisteredService>();
      sut.Should().ThrowExactly<InvalidOperationException>().Where(exception => exception.Message.Contains(nameof(UnregisteredService)));
    }

    [Fact]
    public void when_begin_scope_it_should_return_instance_of_scoped_service()
    {
      var adapter = new SimpleInjectorAdapter(MakeContainer());
      using (adapter.BeginScope())
      {
        adapter.GetService(typeof(ScopedService)).Should().BeOfType<ScopedService>("scoped service should be available in a scope");
        adapter.GetService<ScopedService>().Should().BeOfType<ScopedService>("scoped service should be available in a scope");
      }

      Action sut = () => adapter.GetService<ScopedService>();
      sut.Should().ThrowExactly<InvalidOperationException>("scoped service should be unavailable in a scope");
    }

    [Fact]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void when_construct_with_container_without_configured_default_scoped_lifestyle_it_should_fail()
    {
      Action sut = () => new SimpleInjectorAdapter(MakeContainer(shouldConfigureDefaultScopedLifestyle: false));
      sut.Should().ThrowExactly<InvalidOperationException>(
        "container contains scoped registrations but the default scope lifestyle is not configured");
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void when_construct_with_invalid_arguments_it_should_fail()
    {
      Action sut = () => new SimpleInjectorAdapter(container: null);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    private static Container MakeContainer(bool shouldConfigureDefaultScopedLifestyle = true)
    {
      var container = new Container();
      if (shouldConfigureDefaultScopedLifestyle) container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

      container.Register<IService1, Service1>();
      container.Register<Service1>();
      container.Register<Service2>();
      container.Register<ScopedService>(Lifestyle.Scoped);
      return container;
    }

    private class Service2 { }

    [SuppressMessage("ReSharper", "UnusedType.Local")]
    private class UnregisteredService { }

    private class Service1 : IService1 { }

    private interface IService1 { }

    private class ScopedService { }
  }
}
