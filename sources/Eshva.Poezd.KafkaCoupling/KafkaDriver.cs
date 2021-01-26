#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  public class KafkaDriver : IMessageBrokerDriver
  {
    public Task StartConsumeMessages() => throw new NotImplementedException();

    public Task Publish(byte[] brokerPayload, IReadOnlyDictionary<string, string> brokerMetadata) => throw new NotImplementedException();

    public Task SubscribeToQueues(IEnumerable<string> queueNamePatterns) => throw new NotImplementedException();
  }
}
