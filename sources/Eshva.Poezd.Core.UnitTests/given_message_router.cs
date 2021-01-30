#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eshva.Poezd.Core.Activation;
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

    [Fact]
    public void when_starting_it_should_subscribe_to_all_queues_specified_in_configuration()
    {
      var container = new Container();
      var messageRouter = GetMessageRouter(container);

      var messageBroker = messageRouter.Brokers.Single(broker => broker.Id.Equals("sample-broker-server"));
      ((TestBrokerDriver) messageBroker.Driver).SubscribedQueueNamePatters.Should().BeEquivalentTo(
        new[]
        {
          @"^sample\.(commands|facts)\.service1\.v1",
          "sample.facts.service-2.v1",
          @"^sample\.cdc\..*"
        },
        "it is full list of subscribed queues");
    }

    [Fact]
    public void when_message_received_it_should_create_pipeline_for_its_handling()
    {
      var container = new Container();
      var messageRouter = GetMessageRouter(container);
      messageRouter.RouteIncomingMessage(
        "sample-broker-server",
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

    [Fact]
    public void when_route_message_requested_it_should_router_message_to_all_application_handlers_of_message_from_container()
    {
      var container = new Container();
      var testProperties = new TestProperties();
      container.RegisterInstance(testProperties);
      container.Collection.Register(typeof(ICustomHandler<>), Assembly.GetExecutingAssembly());
      container.Verify();

      ConfigurePoezd(container);

      using var scope = new Scope(container);
      var router = scope.GetInstance<MessageRouter>();

      const string QueueName = "some-topic";
      const string BrokerId = "broker-id";
      router.RouteIncomingMessage(
        BrokerId,
        QueueName,
        DateTimeOffset.UtcNow,
        new byte[0],
        new Dictionary<string, string>());
      router.RouteIncomingMessage(
        BrokerId,
        QueueName,
        DateTimeOffset.UtcNow,
        new byte[0],
        new Dictionary<string, string>());
      router.RouteIncomingMessage(
        BrokerId,
        QueueName,
        DateTimeOffset.UtcNow,
        new byte[0],
        new Dictionary<string, string>());

      testProperties.Handled1.Should()
        .Be(expected: 2, $"there is 2 handlers of {nameof(CustomCommand1)}: {nameof(CustomHandler1)} and {nameof(CustomHandler2)}");
      testProperties.Handled2.Should()
        .Be(
          expected: 3,
          $"there is 3 handlers of {nameof(CustomCommand2)}: {nameof(CustomHandler2)}, " +
          $"{nameof(CustomHandler12)} and {nameof(CustomHandler23)}");
      testProperties.Handled3.Should()
        .Be(expected: 1, $"there is 1 handlers of {nameof(CustomCommand3)}: {nameof(CustomHandler23)}");
    }

    [Fact]
    public void when_route_message_requested_it_should_provide_adopted_context_to_application_message_handlers()
    {
      var container = new Container();
      var testProperties = new TestProperties();
      container.RegisterInstance(testProperties);
      container.Collection.Register(typeof(ICustomHandler<>), Assembly.GetExecutingAssembly());
      container.Verify();

      ConfigurePoezd(container);

      using var scope = new Scope(container);
      var router = scope.GetInstance<MessageRouter>();
      var context = new MessageHandlingContext();
      const string ExpectedProperty1Value = "value1";
      context.Set(CustomHandler1.Property1, ExpectedProperty1Value);

      const string QueueName = "some-topic";
      router.RouteIncomingMessage(
        "broker-id",
        QueueName,
        DateTimeOffset.UtcNow,
        new byte[0],
        new Dictionary<string, string>());

      testProperties.Property1.Should()
        .Be(ExpectedProperty1Value, $"{nameof(CustomHandler1)} set property in own execution context");
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
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.RegisterInstance<IServiceProvider>(container);
      const string TestBrokerDriverSettings = "some setting";
      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker.WithId("sample-broker-server")
                .WithDriver<TestBrokerDriverFactory, TestBrokerDriverConfigurator, TestBrokerDriverConfiguration>(
                  driver => driver.WithSomeSetting(TestBrokerDriverSettings))
                .WithQueueNameMatcher<RegexQueueNameMatcher>()
                .WithIngressEnterPipelineConfigurator<SampleBrokerPipelineConfigurator>()
                .AddPublicApi(
                  api => api.WithId("api-1")
                    .AddQueueNamePattern(@"^sample\.(commands|facts)\.service1\.v1")
                    .WithIngressPipelineConfigurator<Service1PipelineConfigurator>())
                .AddPublicApi(
                  api => api.WithId("api-2")
                    .AddQueueNamePattern("sample.facts.service-2.v1")
                    .WithIngressPipelineConfigurator<Service2PipelineConfigurator>())
                .AddPublicApi(
                  api => api.WithId("cdc-notifications")
                    .AddQueueNamePattern(@"^sample\.cdc\..*")
                    .WithIngressPipelineConfigurator<CdcNotificationsPipelineConfigurator>()))
            .WithMessageHandling(
              messageHandling => messageHandling
                .WithMessageHandlersFactory(new CustomMessageHandlerFactory(container))));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      container.RegisterInstance(GetLoggerFactory());
      container.Register(
        typeof(ILogger<>),
        typeof(Logger<>),
        Lifestyle.Singleton);

      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.Register<SampleBrokerPipelineConfigurator>(Lifestyle.Scoped);
      container.Register<Service1PipelineConfigurator>(Lifestyle.Scoped);
      container.Register<Service2PipelineConfigurator>(Lifestyle.Scoped);
      container.Register<CdcNotificationsPipelineConfigurator>(Lifestyle.Scoped);
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
          .WriteTo.XunitTestOutput(_testOutputHelper)
          .MinimumLevel.Verbose()
          .CreateLogger());

    private readonly ITestOutputHelper _testOutputHelper;
  }
}
