#region Usings

using System.Collections.Generic;
using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public interface IMessageBrokerDriver
  {
    Task StartConsumeMessages();

    Task Publish(byte[] brokerPayload, IReadOnlyDictionary<string, string> brokerMetadata);

    Task Subscribe(IEnumerable<string> queueNamePatterns);
  }
}
