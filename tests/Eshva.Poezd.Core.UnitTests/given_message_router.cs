#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using Eshva.Poezd.SimpleInjectorCoupling;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public sealed class given_message_router
  {
    public given_message_router(ITestOutputHelper testOutputHelper)
    {
      _testOutputHelper = testOutputHelper;
    }
    /*

    [Fact]
    public void when_starting_it_should_subscribe_to_all_queues_specified_in_configuration()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      var messageRouter = GetMessageRouter(container);

      var messageBroker = messageRouter.Brokers.Single(broker => broker.Id.Equals(SampleBrokerServer));
      // TODO: Rework the tests.
      // ((TestBrokerDriver) messageBroker.Driver).SubscribedQueueNamePatters.Should().BeEquivalentTo(
      //   new[]
      //   {
      //     @"^sample\.(commands|facts)\.service1\.v1",
      //     "sample.facts.service-2.v1",
      //     @"^sample\.cdc\..*"
      //   },
      //   "it is full list of subscribed queues");
    }

    [Fact]
    public void when_message_received_it_should_create_pipeline_for_its_handling()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      var messageRouter = GetMessageRouter(container);
      messageRouter.RouteIngressMessage(
        SampleBrokerServer,
        "sample.facts.service-2.v1",
        DateTimeOffset.UtcNow,
        new byte[0],
        new Dictionary<string, string>());

      InMemorySink.Instance.LogEvents
        .Where(@event => @event.Level == LogEventLevel.Information)
        .Select(@event => @event.MessageTemplate.Text)
        .Should().BeEquivalentTo(
          new[]
          {
            nameof(LogMessageHandlingContextStep),
            nameof(Service2DeserializeMessageStep),
            nameof(GetMessageHandlersStep),
            nameof(DispatchMessageToHandlersStep),
            nameof(CommitMessageStep)
          },
          options => options.WithStrictOrdering());
    }

    private IMessageRouter GetMessageRouter(Container container)
    {
      ConfigurePoezd(container);
      var messageRouter = container.GetInstance<IMessageRouter>();
      messageRouter.Start();
      return messageRouter;
    }

    private void ConfigurePoezd(Container container)
    {
      container.RegisterInstance<IServiceProvider>(container);
      const string TestBrokerDriverSettings = "some setting";
      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("venture-kafka")
                .Ingress(
                  ingress => ingress
                    .WithKafkaDriver(
                      driver => driver
                        .WithConsumerConfig(CreateConsumerConfig())
                        .WithHeaderValueParser<Utf8ByteStringHeaderValueParser>())
                    .WithEnterPipeFitter<TIngressEnterPipeline>()
                    .WithExitPipeFitter<TIngressExitPipeline>()
                    .WithQueueNameMatcher<RegexQueueNameMatcher>()
                    .AddPublicApi(
                      api => api
                        .WithId("case-office-ingress")
                        .WithQueueNamePatternsProvider<VentureQueueNamePatternsProvider>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageTypesRegistry<CaseOfficeIngressMessageTypesRegistry>()
                        .WithHandlerRegistry<VentureServiceHandlersRegistry>()))
                .Egress(
                  egress => egress
                    .WithKafkaDriver(
                      driver => driver
                        .WithProducerConfig(CreateProducerConfig()))
                    .WithEnterPipeFitter<TEgressEnterPipeline>()
                    .WithExitPipeFitter<TEgressExitPipeline>()
                    .AddPublicApi(
                      api => api
                        .WithId("case-office-egress")
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageTypesRegistry<CaseOfficeEgressMessageTypesRegistry>()))));
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId(SampleBrokerServer)
                .WithDriver<TestBrokerDriverFactory, TestBrokerDriverConfigurator, TestBrokerDriverConfiguration>(
                  driver => driver.WithSomeSetting(TestBrokerDriverSettings))
                .WithQueueNameMatcher<RegexQueueNameMatcher>()
                .WithIngressEnterPipeFitter<SampleBrokerPipeFitter>()
                .WithIngressExitPipeFitter<EmptyPipeFitter>()
                .WithEgressEnterPipeFitter<EmptyPipeFitter>()
                .WithEgressExitPipeFitter<EmptyPipeFitter>()
                .AddPublicApi(
                  api => api
                    .WithId("api-1")
                    .WithQueueNamePatternsProvider<Service1QueueNamePatternsProvider>()
                    .WithIngressPipeFitter<Service1PipeFitter>()
                    .WithEgressPipeFitter<EmptyPipeFitter>()
                    .WithMessageTypesRegistry<EmptyMessageTypesRegistry>()
                    .WithHandlerRegistry<NoneHandlerRegistry>())
                .AddPublicApi(
                  api => api
                    .WithId("api-2")
                    .WithQueueNamePatternsProvider<Service2QueueNamePatternsProvider>()
                    .WithIngressPipeFitter<Service2PipeFitter>()
                    .WithEgressPipeFitter<EmptyPipeFitter>()
                    .WithMessageTypesRegistry<EmptyMessageTypesRegistry>()
                    .WithHandlerRegistry<NoneHandlerRegistry>())
                .AddPublicApi(
                  api => api
                    .WithId("cdc-notifications")
                    .WithQueueNamePatternsProvider<CdcNotificationsQueueNamePatternsProvider>()
                    .WithIngressPipeFitter<CdcNotificationsPipeFitter>()
                    .WithEgressPipeFitter<EmptyPipeFitter>()
                    .WithMessageTypesRegistry<EmptyMessageTypesRegistry>()
                    .WithHandlerRegistry<NoneHandlerRegistry>())));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      container.RegisterInstance(GetLoggerFactory());
      container.Register(
        typeof(ILogger<>),
        typeof(Logger<>),
        Lifestyle.Singleton);

      container.RegisterSingleton<EmptyMessageTypesRegistry>();
      container.RegisterSingleton<NoneHandlerRegistry>();
      container.RegisterSingleton<TestBrokerDriverFactory>();
      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.RegisterSingleton<Service1QueueNamePatternsProvider>();
      container.RegisterSingleton<Service2QueueNamePatternsProvider>();
      container.RegisterSingleton<CdcNotificationsQueueNamePatternsProvider>();
      container.RegisterSingleton<SampleBrokerPipeFitter>();
      container.RegisterSingleton<EmptyPipeFitter>();
      container.RegisterSingleton<Service1PipeFitter>();
      container.RegisterSingleton<Service2PipeFitter>();
      container.RegisterSingleton<CdcNotificationsPipeFitter>();
      container.Register<LogMessageHandlingContextStep>(Lifestyle.Scoped);
      container.Register<CdcNotificationsCommitStep>(Lifestyle.Scoped);
      container.Register<Service1DeserializeMessageStep>(Lifestyle.Scoped);
      container.Register<Service2DeserializeMessageStep>(Lifestyle.Scoped);
      container.Register<GetMessageHandlersStep>(Lifestyle.Scoped);
      container.Register<DispatchMessageToHandlersStep>(Lifestyle.Scoped);
      container.Register<CommitMessageStep>(Lifestyle.Scoped);

      container.Verify();
    }

    private ILoggerFactory GetLoggerFactory() =>
      new LoggerFactory().AddSerilog(
        new LoggerConfiguration()
          .WriteTo.InMemory()
          .WriteTo.TestOutput(_testOutputHelper)
          .MinimumLevel.Verbose()
          .CreateLogger());
          */

    private readonly ITestOutputHelper _testOutputHelper;
    private const string SampleBrokerServer = "sample-broker-server";
  }
}
