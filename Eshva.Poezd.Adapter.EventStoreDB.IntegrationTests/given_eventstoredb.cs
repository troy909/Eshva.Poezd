#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Adapter.EventStoreDB.Ingress;
using Eshva.Poezd.Adapter.EventStoreDB.IntegrationTests.Tools;
using Eshva.Poezd.Adapter.SimpleInjector;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Venture.Common.TestingTools.Core;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.IntegrationTests
{
  public class given_eventstoredb
  {
    [Fact]
    public async Task when_setup_connection_it_should_handle_messages_from_eventstoredb()
    {
      var doneOrTimeout = Cancellation.TimeoutToken(TimeSpan.FromSeconds(value: 5));

      var isMessageHandled = false;
      var router = CreateConfiguredMessageRouter(() => isMessageHandled = true);
      await router.Start(doneOrTimeout);

      await PublishMessage();
      await doneOrTimeout;

      isMessageHandled.Should().BeTrue("published message should be handled be properly configured message router");
    }

    private IMessageRouter CreateConfiguredMessageRouter(Action messageHandledAction)
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.RegisterSingleton<Utf8ByteStringHeaderValueCodec>();
      container.RegisterSingleton<EmptyPipeFitter>();
      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.RegisterSingleton<TestQueueNamePatternsProvider>();
      container.RegisterSingleton<TestIngressApiMessageTypesRegistry>();
      container.RegisterSingleton<TestServiceHandlersRegistry>();
      container.RegisterSingleton<TestIngressEnterPipeline>();
      container.RegisterSingleton<TestIngressExitPipeline>();

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
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    // .WithEnterPipeFitter<TestIngressEnterPipeline>()
                    // .WithExitPipeFitter<TestIngressExitPipeline>()
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

      // TODO: Call from an exit pipeline step.
      messageHandledAction();

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      return container.GetInstance<IMessageRouter>();
    }

    private Task PublishMessage() => Task.CompletedTask;
  }
}
