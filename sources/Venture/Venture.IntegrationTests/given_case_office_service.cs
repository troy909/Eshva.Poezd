#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
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
    public void when_create_case_command_received_it_should_create_case_in_store()
    {
      var container = RoutingTests.SetupContainer<EmptyPipeFitter, EmptyPipeFitter>(
        api => api.WithId("case-office")
          .WithQueueNamePatternsProvider<PublicApi1QueueNamePatternsProvider>()
          .WithIngressPipelineConfigurator<EmptyPipeFitter>()
          .WithHandlerRegistry<PublicApi1HandlerRegistry>());
      var router = container.GetMessageRouter();
    }

    private KafkaTestContextFactory _kafkaTestContextFactory;
  }
}
