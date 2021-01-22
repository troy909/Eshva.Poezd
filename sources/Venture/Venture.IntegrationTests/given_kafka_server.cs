#region Usings

using System;
using SimpleInjector;
using Venture.Common.TestingTools;
using Xunit;

#endregion


namespace Venture.IntegrationTests
{
  [Collection(KafkaSetupCollection.Name)]
  public sealed class given_kafka_server
  {
    public given_kafka_server(KafkaSetupContainerAsyncFixture fixture)
    {
      _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    [Fact]
    public void when_message_published_to_kafka_topic_it_should_be_received_by_poezd_with_correct_configuration()
    {
      var container = new Container();
      // configure DI-container.
    }

    private readonly KafkaSetupContainerAsyncFixture _fixture;
  }
}
