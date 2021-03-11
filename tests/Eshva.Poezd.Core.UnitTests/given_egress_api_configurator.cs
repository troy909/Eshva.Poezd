#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_egress_api_configurator
  {
    [Fact]
    public void when_id_set_it_should_be_set_in_configuration()
    {
      var configuration = new EgressApiConfiguration();
      var sut = new EgressApiConfigurator(configuration);
      const string expected = "id";
      sut.WithId(expected).Should().BeSameAs(sut);
      configuration.Id.Should().Be(expected);
    }

    [Fact]
    public void when_enter_pipe_fitter_set_it_should_be_set_in_configuration()
    {
      var configuration = new EgressApiConfiguration();
      var sut = new EgressApiConfigurator(configuration);
      sut.WithPipeFitter<PipeFitter>().Should().BeSameAs(sut);
      configuration.PipeFitterType.Should().Be<PipeFitter>();
    }

    [Fact]
    public void when_message_type_registry_set_it_should_be_set_in_configuration()
    {
      var configuration = new EgressApiConfiguration();
      var sut = new EgressApiConfigurator(configuration);
      sut.WithMessageTypesRegistry<MessageTypesRegistry1>().Should().BeSameAs(sut);
      configuration.MessageTypesRegistryType.Should().Be<MessageTypesRegistry1>();
    }

    [Fact]
    public void when_constructed_without_configuration_object_it_should_fail()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new EgressApiConfigurator(configuration: null);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_null_set_as_id_it_should_fail()
    {
      var configurator = new EgressApiConfigurator(new EgressApiConfiguration());
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.WithId(id: null);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_empty_string_set_as_id_it_should_fail()
    {
      var configurator = new EgressApiConfigurator(new EgressApiConfiguration());
      Action sut = () => configurator.WithId(string.Empty);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_whitespace_string_set_as_id_it_should_fail()
    {
      var configurator = new EgressApiConfigurator(new EgressApiConfiguration());
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.WithId(" \n\t");
      sut.Should().Throw<ArgumentNullException>();
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class MessageTypesRegistry1 : IEgressMessageTypesRegistry
    {
      public string GetMessageTypeNameByItsMessageType(Type messageType) => string.Empty;

      public IEgressMessageTypeDescriptor<TMessage> GetDescriptorByMessageType<TMessage>() where TMessage : class => null!;

      public bool DoesOwn<TMessage>() where TMessage : class => false;
    }

    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class HandlerRegistry : IHandlerRegistry
    {
      public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; }
    }

    [UsedImplicitly]
    private class PipeFitter : IPipeFitter
    {
      public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class { }
    }
  }
}
