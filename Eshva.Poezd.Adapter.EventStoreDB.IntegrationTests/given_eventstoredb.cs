#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Adapter.EventStoreDB.Ingress;
using Eshva.Poezd.Adapter.SimpleInjector;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.IntegrationTests
{
  public class given_eventstoredb
  {
    [Fact]
    public void when_setup_connection_it_should_handle_messages_from_eventstoredb()
    {
      var isMessageHandled = false;
      var router = CreateConfiguredMessageRouter(() => isMessageHandled = true);
      PublishMessage();

      isMessageHandled.Should().BeTrue("published message should be handled be properly configured message router");
    }

    private IMessageRouter CreateConfiguredMessageRouter(Action messageHandledAction)
    {
      // TODO: Return message router.

      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

      var connectionConfiguration = new EventStoreDbConnectionConfiguration();
      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("test-eventstoredb-server")
                .Ingress(
                  ingress => ingress
                    .WithEventStoreDbDriver(
                      driver => driver
                        .WithConnection(connectionConfiguration)
                        .WithHeaderValueCodec<Utf8ByteStringHeaderValueCodec>())
                    .WithEnterPipeFitter<TestIngressEnterPipeline>()
                    .WithExitPipeFitter<TestIngressExitPipeline>()
                    .WithQueueNameMatcher<RegexQueueNameMatcher>()
                    .AddApi(
                      api => api
                        .WithId("ingress-case-office")
                        .WithQueueNamePatternsProvider<TestQueueNamePatternsProvider>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<TestIngressApiMessageTypesRegistry>()
                        .WithHandlerRegistry<TestServiceHandlersRegistry>()
                    ))
                .WithoutEgress()));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      return container.GetInstance<IMessageRouter>();
    }

    private void PublishMessage() { }
  }

  internal class TestServiceHandlersRegistry : IHandlerRegistry
  {
    public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; }
  }

  internal class TestIngressApiMessageTypesRegistry : IIngressApiMessageTypesRegistry
  {
    public Type GetMessageTypeByItsMessageTypeName(string messageTypeName) => throw new NotImplementedException();

    public IIngressMessageTypeDescriptor<TMessage> GetDescriptorByMessageTypeName<TMessage>(string messageTypeName)
      where TMessage : class => throw new NotImplementedException();
  }

  internal class TestQueueNamePatternsProvider : IQueueNamePatternsProvider
  {
    public IEnumerable<string> GetQueueNamePatterns() => throw new NotImplementedException();
  }

  internal class TestIngressExitPipeline : IPipeFitter
  {
    public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class
    {
      throw new NotImplementedException();
    }
  }

  internal class TestIngressEnterPipeline : IPipeFitter
  {
    public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Pipe fitter producing no steps.
  /// </summary>
  [ExcludeFromCodeCoverage]
  internal class EmptyPipeFitter : IPipeFitter
  {
    /// <inheritdoc />
    public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class { }
  }
}
