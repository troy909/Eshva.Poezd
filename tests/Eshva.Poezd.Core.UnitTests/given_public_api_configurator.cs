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
  public class given_public_api_configurator
  {
    [Fact]
    public void when_id_set_it_should_be_set_in_configuration()
    {
      var configuration = new PublicApiConfiguration();
      var sut = new PublicApiConfigurator(configuration);
      const string expected = "id";
      sut.WithId(expected).Should().BeSameAs(sut);
      configuration.Id.Should().Be(expected);
    }

    [Fact]
    public void when_ingress_enter_pipe_fitter_set_it_should_be_set_in_configuration()
    {
      var configuration = new PublicApiConfiguration();
      var sut = new PublicApiConfigurator(configuration);
      sut.WithIngressPipeFitter<PipeFitter>().Should().BeSameAs(sut);
      configuration.IngressPipeFitterType.Should().Be<PipeFitter>();
    }

    [Fact]
    public void when_handler_registry_set_it_should_be_set_in_configuration()
    {
      var configuration = new PublicApiConfiguration();
      var sut = new PublicApiConfigurator(configuration);
      sut.WithHandlerRegistry<HandlerRegistry>().Should().BeSameAs(sut);
      configuration.HandlerRegistryType.Should().Be<HandlerRegistry>();
    }

    [Fact]
    public void when_queue_name_patterns_provider_set_it_should_be_set_in_configuration()
    {
      var configuration = new PublicApiConfiguration();
      var sut = new PublicApiConfigurator(configuration);
      sut.WithQueueNamePatternsProvider<QueueNamePatternsProvider>().Should().BeSameAs(sut);
      configuration.QueueNamePatternsProviderType.Should().Be<QueueNamePatternsProvider>();
    }

    [Fact]
    public void when_message_type_registry_set_it_should_be_set_in_configuration()
    {
      var configuration = new PublicApiConfiguration();
      var sut = new PublicApiConfigurator(configuration);
      sut.WithMessageTypesRegistry<MessageTypesRegistry1>().Should().BeSameAs(sut);
      configuration.MessageTypesRegistryType.Should().Be<MessageTypesRegistry1>();
    }

    [Fact]
    public void when_constructed_without_configuration_object_it_should_fail()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new PublicApiConfigurator(configuration: null);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_null_set_as_id_it_should_fail()
    {
      var configurator = new PublicApiConfigurator(new PublicApiConfiguration());
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.WithId(id: null);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_empty_string_set_as_id_it_should_fail()
    {
      var configurator = new PublicApiConfigurator(new PublicApiConfiguration());
      Action sut = () => configurator.WithId(string.Empty);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_whitespace_string_set_as_id_it_should_fail()
    {
      var configurator = new PublicApiConfigurator(new PublicApiConfiguration());
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.WithId(" \n\t");
      sut.Should().Throw<ArgumentNullException>();
    }

    public class MessageTypesRegistry1 { }

    public class QueueNamePatternsProvider : IQueueNamePatternsProvider
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
