#region Usings

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.Client;
using FluentAssertions;
using RandomStringCreator;
using Venture.Common.TestingTools.Core;
using Venture.Common.TestingTools.EventStoreDb;
using Xunit;

#endregion

namespace Venture.Common.TestingTools.IntegrationTests
{
  [Collection(EventStoreSetupCollection.Name)]
  public class given_event_store_test_context
  {
    public given_event_store_test_context(EventStoreSetupContainerAsyncFixture fixture)
    {
      _fixture = fixture;
      _client = new EventStoreClient(EventStoreClientSettings.Create(@$"esdb://{HostName}:{_fixture.ExposedHttpPort}?tls={IsSecure}"));
    }

    [Fact]
    public async Task when_append_to_stream_it_should_append_event_to_specified_stream()
    {
      var timeout = Cancellation.TimeoutToken(TimeSpan.FromSeconds(value: 5));
      var testContext = new EventStoreTestContext(HostName, _fixture.ExposedHttpPort);

      const string eventPayload = "event payload";
      const string eventType = "Create";
      const string eventMetadata = "event metadata";
      var @event = new EventData(
        Uuid.NewUuid(),
        eventType,
        Encoding.UTF8.GetBytes(eventPayload),
        Encoding.UTF8.GetBytes(eventMetadata));
      var streamName = RandomString();

      await testContext.AppendToStream(
        @event,
        streamName,
        timeout);

      var result = _client.ReadStreamAsync(
        Direction.Forwards,
        streamName,
        StreamPosition.Start);
      var events = await result.ToListAsync(timeout);
      events.Should().HaveCount(expected: 1, "a single event has been appended");
      var eventRecord = events.Single().Event;
      Encoding.UTF8.GetString(eventRecord.Data.Span).Should().Be(eventPayload);
      Encoding.UTF8.GetString(eventRecord.Metadata.Span).Should().Be(eventMetadata);
      eventRecord.EventType.Should().Be(eventType);
    }

    private static string RandomString(uint length = 10) => new StringCreator().Get((int) length);

    private readonly EventStoreClient _client;
    private readonly EventStoreSetupContainerAsyncFixture _fixture;

    private const bool IsSecure = false;
    private const string HostName = "localhost";
  }
}
