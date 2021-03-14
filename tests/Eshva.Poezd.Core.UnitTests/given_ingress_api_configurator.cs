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
  public class given_ingress_api_configurator
  {
    [Fact]
    public void when_id_set_it_should_be_set_in_configuration()
    {
      var configuration = new IngressApiConfiguration();
      var sut = new IngressApiConfigurator(configuration);
      const string expected = "id";
      sut.WithId(expected).Should().BeSameAs(sut);
      configuration.Id.Should().Be(expected);
    }

    [Fact]
    public void when_key_type_set_it_should_be_set_in_configuration()
    {
      var configuration = new IngressApiConfiguration();
      var sut = new IngressApiConfigurator(configuration);
      sut.WithMessageKey<int>().Should().BeSameAs(sut);
      configuration.MessageKeyType.Should().Be<int>();
    }

    [Fact]
    public void when_payload_type_set_it_should_be_set_in_configuration()
    {
      var configuration = new IngressApiConfiguration();
      var sut = new IngressApiConfigurator(configuration);
      sut.WithMessagePayload<string>().Should().BeSameAs(sut);
      configuration.MessagePayloadType.Should().Be<string>();
    }

    [Fact]
    public void when_enter_pipe_fitter_set_it_should_be_set_in_configuration()
    {
      var configuration = new IngressApiConfiguration();
      var sut = new IngressApiConfigurator(configuration);
      sut.WithPipeFitter<PipeFitter>().Should().BeSameAs(sut);
      configuration.PipeFitterType.Should().Be<PipeFitter>();
    }

    [Fact]
    public void when_handler_registry_set_it_should_be_set_in_configuration()
    {
      var configuration = new IngressApiConfiguration();
      var sut = new IngressApiConfigurator(configuration);
      sut.WithHandlerRegistry<HandlerRegistry>().Should().BeSameAs(sut);
      configuration.HandlerRegistryType.Should().Be<HandlerRegistry>();
    }

    [Fact]
    public void when_queue_name_patterns_provider_set_it_should_be_set_in_configuration()
    {
      var configuration = new IngressApiConfiguration();
      var sut = new IngressApiConfigurator(configuration);
      sut.WithQueueNamePatternsProvider<QueueNamePatternsProvider>().Should().BeSameAs(sut);
      configuration.QueueNamePatternsProviderType.Should().Be<QueueNamePatternsProvider>();
    }

    [Fact]
    public void when_message_type_registry_set_it_should_be_set_in_configuration()
    {
      var configuration = new IngressApiConfiguration();
      var sut = new IngressApiConfigurator(configuration);
      sut.WithMessageTypesRegistry<MessageTypesRegistry1>().Should().BeSameAs(sut);
      configuration.MessageTypesRegistryType.Should().Be<MessageTypesRegistry1>();
    }

    [Fact]
    public void when_constructed_without_configuration_object_it_should_fail()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new IngressApiConfigurator(configuration: null);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_null_set_as_id_it_should_fail()
    {
      var configurator = new IngressApiConfigurator(new IngressApiConfiguration());
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.WithId(id: null);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_empty_string_set_as_id_it_should_fail()
    {
      var configurator = new IngressApiConfigurator(new IngressApiConfiguration());
      Action sut = () => configurator.WithId(string.Empty);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_whitespace_string_set_as_id_it_should_fail()
    {
      var configurator = new IngressApiConfigurator(new IngressApiConfiguration());
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.WithId(" \n\t");
      sut.Should().Throw<ArgumentNullException>();
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class MessageTypesRegistry1 : IIngressMessageTypesRegistry
    {
      public Type GetMessageTypeByItsMessageTypeName(string messageTypeName) => typeof(object);

      public IIngressMessageTypeDescriptor<TMessage> GetDescriptorByMessageTypeName<TMessage>(
        string messageTypeName) where TMessage : class =>
        null!;
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class QueueNamePatternsProvider : IQueueNamePatternsProvider
    {
      public IEnumerable<string> GetQueueNamePatterns() => throw new NotImplementedException();
    }

    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class HandlerRegistry : IHandlerRegistry
    {
      public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; }
    }

    [UsedImplicitly]
    private class PipeFitter : IPipeFitter
    {
      public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class
      {
        throw new NotImplementedException();
      }
    }
  }
}
