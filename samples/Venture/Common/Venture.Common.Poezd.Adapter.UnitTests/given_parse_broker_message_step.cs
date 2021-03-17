#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Routing;
using FlatSharp;
using FluentAssertions;
using Venture.CaseOffice.Messages;
using Venture.CaseOffice.Messages.V1.Commands;
using Venture.Common.Poezd.Adapter.Ingress;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_parse_broker_message_step
  {
    [Fact]
    public async Task when_executed_with_broker_message_and_message_type_name_withing_context_it_should_store_deserialized_message()
    {
      var (message, messageBytes) = CreateSerializedMessage();
      var registry = new CaseOfficeIngressMessageTypesRegistry();
      registry.Initialize();

      var messageType = typeof(CreateJusticeCase);
      var context = new MessageHandlingContext
      {
        Payload = messageBytes,
        MessageType = messageType,
        Descriptor = registry.GetDescriptorByMessageTypeName<CreateJusticeCase>(messageType.FullName!)
      };

      var sut = new ParseBrokerMessageStep();
      await sut.Execute(context);

      var parsedMessage = context.Message;
      parsedMessage.Should().NotBeSameAs(message, "parsed message should not be the same object as the original");
      parsedMessage.Should().BeEquivalentTo(message, "parsed message should have same property values as the original");
    }

    [Fact]
    public void when_executed_with_broker_message_but_without_message_type_within_context_it_should_fail()
    {
      var (_, messageBytes) = CreateSerializedMessage();
      var registry = new CaseOfficeIngressMessageTypesRegistry();
      registry.Initialize();

      var messageType = typeof(CreateJusticeCase);
      var context = new MessageHandlingContext
      {
        Payload = messageBytes,
        Descriptor = registry.GetDescriptorByMessageTypeName<CreateJusticeCase>(messageType.FullName!)
      };

      var step = new ParseBrokerMessageStep();
      Func<Task> sut = () => step.Execute(context);

      sut.Should().Throw<KeyNotFoundException>("impossible to parse message with its type unknown");
    }

    [Fact]
    public void when_executed_with_message_type_but_without_broker_message_within_context_it_should_fail()
    {
      var registry = new CaseOfficeIngressMessageTypesRegistry();
      registry.Initialize();

      var messageType = typeof(CreateJusticeCase);
      var context = new MessageHandlingContext
      {
        MessageType = messageType,
        Descriptor = registry.GetDescriptorByMessageTypeName<CreateJusticeCase>(messageType.FullName!)
      };

      var step = new ParseBrokerMessageStep();
      Func<Task> sut = () => step.Execute(context);

      sut.Should().Throw<KeyNotFoundException>("impossible to parse message without its payload");
    }

    [Fact]
    public void when_executed_with_message_type_and_message_payload_but_without_message_type_descriptor_within_context_it_should_fail()
    {
      var (_, messageBytes) = CreateSerializedMessage();
      var registry = new CaseOfficeIngressMessageTypesRegistry();
      registry.Initialize();

      var messageType = typeof(CreateJusticeCase);
      var context = new MessageHandlingContext {MessageType = messageType, Payload = messageBytes};

      var step = new ParseBrokerMessageStep();
      Func<Task> sut = () => step.Execute(context);

      sut.Should().Throw<KeyNotFoundException>("impossible to parse message without its message type descriptor");
    }

    [Fact]
    public void when_executed_without_context_it_should_fail()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = async () =>
        await new ParseBrokerMessageStep().Execute(context: null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("context"), "context must be specified");
    }

    private static (CreateJusticeCase, byte[]) CreateSerializedMessage()
    {
      var message = new CreateJusticeCase {SubjectId = Guid.NewGuid(), Reason = "some reason", ResponsibleId = Guid.NewGuid()};
      var buffer = new byte[1024];
      FlatBufferSerializer.Default.Serialize(message, buffer);
      return (message, buffer);
    }
  }
}
