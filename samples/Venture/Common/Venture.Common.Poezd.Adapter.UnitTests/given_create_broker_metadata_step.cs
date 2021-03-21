#region Usings

using System;
using System.Collections.Generic;
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
  public class given_create_broker_metadata_step
  {
    [Fact]
    public void when_executed_it_should_store_message_metadata_into_context()
    {
      var sut = new GetBrokerMetadataStep();
      var context = new MessagePublishingContext
      {
        Api = CreateEgressApi(),
        Message = new Message1(),
        CorrelationId = ExpectedCorrelationId,
        CausationId = ExpectedCausationId,
        MessageId = ExpectedMessageId
      };

      sut.Execute(context);

      var expectedMetadata = new Dictionary<string, string>
      {
        {VentureApi.Headers.MessageTypeName, ExpectedMessageTypeName},
        {VentureApi.Headers.CorrelationId, ExpectedCorrelationId},
        {VentureApi.Headers.CausationId, ExpectedCausationId},
        {VentureApi.Headers.MessageId, ExpectedMessageId}
      };
      context.Metadata.Should().BeEquivalentTo(expectedMetadata, "it should store metadata with expected content");
    }

    [Fact]
    public void when_executing_without_context_it_should_fail()
    {
      var step = new GetBrokerMetadataStep();
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = () => step.Execute(context: null);

      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("context"));
    }

    private static IEgressApi CreateEgressApi()
    {
      var registryMock = new Mock<IEgressApiMessageTypesRegistry>();
      registryMock.Setup(registry => registry.GetMessageTypeNameByItsMessageType(It.IsAny<Type>())).Returns(ExpectedMessageTypeName);
      var egressApiMock = new Mock<IEgressApi>();
      egressApiMock.SetupGet(api => api.MessageTypesRegistry).Returns(() => registryMock.Object);
      return egressApiMock.Object;
    }

    private const string ExpectedMessageTypeName = nameof(ExpectedMessageTypeName);
    private const string ExpectedCorrelationId = nameof(ExpectedCorrelationId);
    private const string ExpectedCausationId = nameof(ExpectedCausationId);
    private const string ExpectedMessageId = nameof(ExpectedMessageId);

    // ReSharper disable once MemberCanBePrivate.Global
    public class Message1 { }
  }
}
