#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using SimpleInjector;
using Venture.CaseOffice.Application;
using Venture.CaseOffice.Messages;
using Venture.CaseOffice.Messages.V1.Commands;
using Venture.CaseOffice.WorkPlanner.Adapter;
using Venture.Common.Poezd.Adapter;
using Venture.Common.TestingTools.Kafka;
using Venture.IntegrationTests.TestSubjects;
using Xunit;

#endregion

namespace Venture.IntegrationTests
{
  [Collection(KafkaSetupCollection.Name)]
  public class given_case_office_service
  {
    public given_case_office_service(KafkaSetupContainerAsyncFixture fixture)
    {
      if (fixture == null) throw new ArgumentNullException(nameof(fixture));

      _kafkaTestContextFactory = new KafkaTestContextFactory(fixture.KafkaContainerConfiguration.BootstrapServers);
    }

    [Fact]
    public async Task when_create_case_command_received_it_should_create_case_in_store()
    {
      var container = RoutingTests.SetupContainer<EmptyPipeFitter, FinishTestPipeFitter>(
        api => api
          .WithId("case-office")
          .WithQueueNamePatternsProvider<VentureQueueNamePatternsProvider>()
          .WithIngressPipelineConfigurator<EmptyPipeFitter>()
          // .WithIngressPipelineConfigurator<IngressPipeFitter>()
          .WithHandlerRegistry<VentureServiceHandlersRegistry>());
      container.RegisterSingleton<VentureQueueNamePatternsProvider>();
      AddIngressPipeline(container);
      container.RegisterInstance(CreateMessageTypeRegistry());

      const string topic = "venture.commands.case-office.v1";
      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<byte[]>(timeout);
      await kafkaTestContext.CreateTopics(topic);

      var testIsFinished = RoutingTests.AddTestFinishSemaphore(container);

      var router = container.GetMessageRouter();
      await router.Start(timeout);

      var command = new CreateCase {CaseId = Guid.NewGuid(), CaseType = "law", Reason = "some reason", SubjectId = Guid.NewGuid()};
      var serialized = container.GetInstance<MessageTypesRegistry>().Serialize(command.GetType().FullName!, command);
      var headers = CreateHeaders(command.GetType());
      await kafkaTestContext.Produce(
        topic,
        serialized,
        headers);

      await testIsFinished.WaitAsync(timeout);
      // TODO: assert
    }

    private static void AddIngressPipeline(Container container)
    {
      // TODO: I need to test this pipeline itself because for the moment it fails.
      /*
      container.RegisterSingleton<IngressPipeFitter>();
      container.Register<ExtractRelationMetadataStep>(Lifestyle.Scoped);
      container.Register<ExtractMessageTypeStep>(Lifestyle.Scoped);
      container.Register<DeserializeMessageStep>(Lifestyle.Scoped);
      container.Register<ExtractAuthorizationMetadataStep>(Lifestyle.Scoped);
      container.Register<FindMessageHandlersStep>(Lifestyle.Scoped);
      container.Register<ExecuteMessageHandlersStep>(Lifestyle.Scoped);

      var handlersRegistry = new VentureServiceHandlersRegistry(new[] {typeof(AssemblyTag).Assembly});
      container.RegisterInstance(handlersRegistry);
      */
      /*
      var handlers = handlersRegistry.HandlersGroupedByMessageType[typeof(CreateCase)];
      foreach (var handler in handlers)
      {
        container.Register(handler);
      }
    */
    }

    private static MessageTypesRegistry CreateMessageTypeRegistry()
    {
      var messagesAssembly = Assembly.GetAssembly(typeof(Api));
      var messageTypes = messagesAssembly!.ExportedTypes.Where(type => type.Namespace!.StartsWith(Api.V1Namespace));
      return new MessageTypesRegistry(messageTypes);
    }

    private static Dictionary<string, byte[]> CreateHeaders(Type messageType)
    {
      var headers = new Dictionary<string, byte[]>
      {
        {Api.Headers.MessageTypeName, StringToBytes(messageType.FullName)},
        {Api.Headers.MessageId, StringToBytes(Guid.NewGuid().ToString("N"))},
        {Api.Headers.CorrelationId, StringToBytes(Guid.NewGuid().ToString("N"))},
        {Api.Headers.CausationId, StringToBytes(Guid.NewGuid().ToString("N"))}
      };
      return headers;
    }

    private static byte[] StringToBytes(string value) => Encoding.UTF8.GetBytes(value);

    private readonly KafkaTestContextFactory _kafkaTestContextFactory;
  }
}
