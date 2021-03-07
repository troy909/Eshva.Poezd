#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Moq;
using Venture.Common.Poezd.Adapter.MessagePublishing;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_serialize_message_step
  {
    [Fact]
    public async Task when_executing_it_should_serialize_message_into_byte_array()
    {
      const byte expected = 0x23;
      var context = new MessagePublishingContext {PublicApi = CreatePublicApi(), Message = new Message1 {Byte = expected}};
      var sut = new SerializeMessageStep();

      await sut.Execute(context);

      context.Payload[0].Should().Be(expected, "message content should be serialized");
    }

    [Fact]
    public void when_executing_without_context_it_should_fail()
    {
      var step = new SerializeMessageStep();
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = () => step.Execute(context: null);

      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("context"));
    }

    private static IEgressPublicApi CreatePublicApi()
    {
      var descriptorMock = new Mock<IEgressMessageTypeDescriptor<Message1>>();
      descriptorMock.Setup(descriptor => descriptor.Serialize(It.IsAny<Message1>(), It.IsAny<Memory<byte>>()))
        .Callback<Message1, Memory<byte>>((message, buffer1) => { buffer1.Span[index: 0] = message.Byte; })
        .Returns(value: 777);
      var registryMock = new Mock<IEgressMessageTypesRegistry>();
      registryMock.Setup(registry => registry.GetDescriptorByMessageType<Message1>()).Returns(descriptorMock.Object);
      var publicApiMock = new Mock<IEgressPublicApi>();
      publicApiMock.SetupGet(api => api.MessageTypesRegistry).Returns(() => registryMock.Object);
      return publicApiMock.Object;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public class Message1
    {
      public byte Byte;
    }
  }
}
