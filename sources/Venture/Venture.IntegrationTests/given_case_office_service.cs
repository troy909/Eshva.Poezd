#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
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
        api => api.WithId("case-office")
          .WithQueueNamePatternsProvider<VentureQueueNamePatternsProvider>()
          .WithIngressPipelineConfigurator<IngressPipeFitter>()
          .WithHandlerRegistry<VentureServiceHandlersRegistry>());
      container.RegisterSingleton<VentureQueueNamePatternsProvider>();
      container.RegisterSingleton<IngressPipeFitter>();
      container.RegisterSingleton<VentureServiceHandlersRegistry>();

      const string topic = "venture.commands.case-office.v1";
      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<string>(timeout);
      await kafkaTestContext.CreateTopics(topic);

      var testIsFinished = RoutingTests.AddTestFinishSemaphore(container);

      var router = container.GetMessageRouter();
      await router.Start(timeout);

      
      await testIsFinished.WaitAsync(timeout);
      // TODO: assert
    }

    private readonly KafkaTestContextFactory _kafkaTestContextFactory;
  }
}
