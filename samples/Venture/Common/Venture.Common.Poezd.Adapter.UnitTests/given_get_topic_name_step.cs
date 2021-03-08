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
  public class given_get_topic_name_step
  {
    [Fact]
    public void when_executed_it_should_store_queue_name_into_context()
    {
      var context = new MessagePublishingContext {Api = CreateEgressApi(), Message = new Message1()};
      var sut = new GetTopicNameStep();
      sut.Execute(context);

      context.QueueNames.Should().BeEquivalentTo(new[] {ExpectedQueueName}, "queue name should be set after step executed");
    }

    [Fact]
    public void when_executing_without_context_it_should_fail()
    {
      var step = new GetTopicNameStep();
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = () => step.Execute(context: null);

      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("context"));
    }

    private static IEgressApi CreateEgressApi()
    {
      var descriptorMock = new Mock<IEgressMessageTypeDescriptor<Message1>>();
      descriptorMock.SetupGet(descriptor => descriptor.QueueNames).Returns(() => new[] {ExpectedQueueName});
      var registryMock = new Mock<IEgressMessageTypesRegistry>();
      registryMock.Setup(registry => registry.GetDescriptorByMessageType<Message1>()).Returns(descriptorMock.Object);
      var egressApiMock = new Mock<IEgressApi>();
      egressApiMock.SetupGet(api => api.MessageTypesRegistry).Returns(() => registryMock.Object);
      return egressApiMock.Object;
    }

    private const string ExpectedQueueName = nameof(ExpectedQueueName);

    // ReSharper disable once MemberCanBePrivate.Global
    public class Message1 { }
  }
}
