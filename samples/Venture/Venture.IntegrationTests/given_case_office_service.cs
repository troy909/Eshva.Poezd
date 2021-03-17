#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using FluentAssertions;
using RandomStringCreator;
using SimpleInjector;
using Venture.CaseOffice.Application;
using Venture.CaseOffice.Domain;
using Venture.CaseOffice.Messages;
using Venture.CaseOffice.Messages.V1.Commands;
using Venture.Common.Application.Storage;
using Venture.Common.Poezd.Adapter;
using Venture.Common.Poezd.Adapter.Ingress;
using Venture.Common.TestingTools.Kafka;
using Venture.IntegrationTests.TestSubjects;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Venture.IntegrationTests
{
  [Collection(KafkaSetupCollection.Name)]
  public class given_case_office_service
  {
    public given_case_office_service(KafkaSetupContainerAsyncFixture fixture, ITestOutputHelper testOutput)
    {
      if (fixture == null) throw new ArgumentNullException(nameof(fixture));
      _testOutput = testOutput;

      _kafkaTestContextFactory = new KafkaTestContextFactory(fixture.KafkaContainerConfiguration.BootstrapServers);
    }

    [Fact]
    public async Task when_create_case_command_received_it_should_create_case_in_store()
    {
      var container = RoutingTests.SetupContainer<EmptyPipeFitter, FinishTestPipeFitter, EmptyPipeFitter, FinishTestPipeFitter>(
        api => api
          .WithId("ingress-case-office")
          .WithQueueNamePatternsProvider<VentureQueueNamePatternsProvider>()
          .WithPipeFitter<VentureIngressPipeFitter>()
          .WithMessageKey<Ignore>()
          .WithMessagePayload<byte[]>()
          .WithMessageTypesRegistry<CaseOfficeIngressMessageTypesRegistry>()
          .WithHandlerRegistry<VentureServiceHandlersRegistry>(),
        api => api
          .WithId("egress-case-office")
          .WithMessageKey<int>()
          .WithMessagePayload<byte[]>()
          .WithMessageTypesRegistry<EmptyEgressMessageTypesRegistry>()
          .WithPipeFitter<EmptyPipeFitter>(),
        _testOutput);
      container.RegisterSingleton<VentureQueueNamePatternsProvider>();
      AddIngressPipeline(container);
      var registry = new CaseOfficeIngressMessageTypesRegistry();
      registry.Initialize();
      container.RegisterInstance(registry);

      const string topic = "venture.commands.case-office.v1";
      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<byte[]>(timeout);
      await kafkaTestContext.CreateTopics(topic);

      var testIsFinished = RoutingTests.AddTestFinishSemaphore(container);

      var router = container.GetMessageRouter();
      await router.Start(timeout);

      var stringCreator = new StringCreator();
      var expectedReason = stringCreator.Get(length: 10);
      var (command, serialized) = CreateSerializedMessage(expectedReason);
      var headers = CreateHeaders(command.GetType());

      await kafkaTestContext.Produce(
        topic,
        serialized,
        headers);

      await testIsFinished.WaitAsync(timeout);
      var justiceCaseStorage = (IAggregateStorageTestBackdoor<JusticeCase>) container.GetInstance<IAggregateStorage<JusticeCase>>();
      justiceCaseStorage.GetAll().Should().HaveCount(expected: 1);
      justiceCaseStorage.GetAll().Single().Reason.Should().Be(expectedReason);
      // TODO: assert
    }

    private static (CreateJusticeCase, byte[]) CreateSerializedMessage(string expectedReason)
    {
      var registry = new CaseOfficeEgressMessageTypesRegistry();
      registry.Initialize();
      var descriptor = registry.GetDescriptorByMessageType<CreateJusticeCase>();
      var serialized = new byte[1024];
      var message = new CreateJusticeCase {Reason = expectedReason, SubjectId = Guid.NewGuid(), ResponsibleId = Guid.NewGuid()};
      descriptor.Serialize(message, serialized);
      return (message, serialized);
    }

    private static void AddIngressPipeline(Container container)
    {
      // TODO: I need to test this pipeline itself because for the moment it fails.
      container.RegisterSingleton<VentureIngressPipeFitter>();
      container.RegisterSingleton<EmptyEgressMessageTypesRegistry>();
      container.Register<IHandlersExecutionStrategy, ParallelHandlersExecutionStrategy>();
      container.RegisterSingleton<IAggregateStorage<ResearchCase>, AlmostRealAggregateStorage<ResearchCase>>();
      container.RegisterSingleton<IAggregateStorage<JusticeCase>, AlmostRealAggregateStorage<JusticeCase>>();
      container.Register<ExtractRelationMetadataStep>(Lifestyle.Scoped);
      container.Register<ExtractMessageTypeStep>(Lifestyle.Scoped);
      container.Register<ParseBrokerMessageStep>(Lifestyle.Scoped);
      container.Register<FindMessageHandlersStep>(Lifestyle.Scoped);
      container.Register<ExecuteMessageHandlersStep>(Lifestyle.Scoped);

      var handlersRegistry = new VentureServiceHandlersRegistry(new[] {typeof(AssemblyTag).Assembly});
      container.RegisterInstance(handlersRegistry);
      var handlers = handlersRegistry.HandlersGroupedByMessageType[typeof(CreateJusticeCase)];
      foreach (var handler in handlers)
      {
        container.Register(handler);
      }
    }

    private static Dictionary<string, byte[]> CreateHeaders(Type messageType)
    {
      var headers = new Dictionary<string, byte[]>
      {
        {VentureApi.Headers.MessageTypeName, StringToBytes(messageType.FullName)},
        {VentureApi.Headers.MessageId, StringToBytes(Guid.NewGuid().ToString("N"))},
        {VentureApi.Headers.CorrelationId, StringToBytes(Guid.NewGuid().ToString("N"))},
        {VentureApi.Headers.CausationId, StringToBytes(Guid.NewGuid().ToString("N"))}
      };
      return headers;
    }

    private static byte[] StringToBytes(string value) => Encoding.UTF8.GetBytes(value);

    private readonly KafkaTestContextFactory _kafkaTestContextFactory;
    private readonly ITestOutputHelper _testOutput;
  }
}
