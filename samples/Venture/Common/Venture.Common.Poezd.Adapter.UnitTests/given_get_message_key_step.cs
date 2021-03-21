#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Moq;
using Venture.Common.Poezd.Adapter.Egress;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_get_message_key_step
  {
    [Fact]
    public void when_executed_it_should_store_message_key_in_context()
    {
      const byte expected = 0x23;
      var context = new MessagePublishingContext {Api = CreateEgressApi(), Message = new Message1 {Byte = expected}};
      var sut = new GetMessageKeyStep();

      sut.Execute(context);

      context.Key.Should().Be(expected, "key should be set after step executed");
    }

    [Fact]
    public void when_executing_without_context_it_should_fail()
    {
      var step = new GetMessageKeyStep();
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = () => step.Execute(context: null);

      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("context"));
    }

    private static IEgressApi CreateEgressApi()
    {
      var descriptorMock = new Mock<IEgressApiMessageTypeDescriptor<Message1>>();
      descriptorMock.Setup(descriptor => descriptor.GetKey(It.IsAny<Message1>())).Returns((Message1 message) => message.Byte);
      var registryMock = new Mock<IEgressApiMessageTypesRegistry>();
      registryMock.Setup(registry => registry.GetDescriptorByMessageType<Message1>()).Returns(descriptorMock.Object);
      var egressApiMock = new Mock<IEgressApi>();
      egressApiMock.SetupGet(api => api.MessageTypesRegistry).Returns(() => registryMock.Object);
      return egressApiMock.Object;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public class Message1
    {
      public byte Byte;
    }
  }
}
