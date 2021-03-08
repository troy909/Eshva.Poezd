#region Usings

using System.Buffers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class TestBrokerEgressApiStep : IStep<MessagePublishingContext>
  {
    public TestBrokerEgressApiStep(ILogger<TestBrokerEgressApiStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(MessagePublishingContext context)
    {
      var registry = context.PublicApi.MessageTypesRegistry;
      var descriptor = registry.GetDescriptorByMessageType<TestEgressMessage1>();
      var message = (TestEgressMessage1) context.Message;

      using var memoryOwner = MemoryPool<byte>.Shared.Rent();
      var buffer = memoryOwner.Memory;
      descriptor.Serialize(message, buffer);

      context.Key = descriptor.GetKey(message);
      context.QueueNames = descriptor.QueueNames;
      context.Payload = buffer.ToArray();
      context.Metadata = new Dictionary<string, string>
      {
        {"Type", registry.GetMessageTypeNameByItsMessageType(typeof(TestEgressMessage1))}
      };

      _logger.LogInformation(nameof(TestBrokerEgressApiStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<TestBrokerEgressApiStep> _logger;
  }
}
