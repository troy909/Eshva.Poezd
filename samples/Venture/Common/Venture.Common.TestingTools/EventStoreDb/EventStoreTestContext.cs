#region Usings

using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

#endregion

namespace Venture.Common.TestingTools.EventStoreDb
{
  public class EventStoreTestContext
  {
    public EventStoreTestContext(
      string hostName = "localhost",
      ushort port = 2113,
      bool isSecure = false)
    {
      _settings = EventStoreClientSettings.Create(@$"esdb://{hostName}:{port}?tls={isSecure}");
    }

    public Task<IWriteResult> AppendToStream(
      EventData @event,
      string streamName,
      CancellationToken cancellationToken)
    {
      EnsureClient();

      return _client.AppendToStreamAsync(
        streamName,
        StreamState.NoStream,
        new[] {@event},
        cancellationToken: cancellationToken);
    }

    private void EnsureClient()
    {
      if (_client != null) return;

      _client = new EventStoreClient(_settings);
    }

    private readonly EventStoreClientSettings _settings;
    private EventStoreClient _client;
  }
}
