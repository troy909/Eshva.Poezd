#region Usings

using System;
using System.Buffers;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using FluentAssertions;
using Venture.CaseOffice.Messages.V1.Commands;
using Xunit;

#endregion

namespace Venture.CaseOffice.Messages.UnitTests
{
  public class given_message_types_registry
  {
    [Fact]
    public void when_registry_initialized_and_clr_type_for_known_message_type_requested_it_should_return_clr_type()
    {
      var sut = new CaseOfficeIngressApiMessageTypesRegistry();
      sut.Initialize();
      sut.GetMessageTypeByItsMessageTypeName(typeof(CreateJusticeCase).FullName!).Should().NotBeNull("type is known");
    }

    [Fact]
    public void when_registry_initialized_and_descriptor_for_known_message_type_requested_it_should_return_descriptor()
    {
      var sut = new CaseOfficeIngressApiMessageTypesRegistry();
      sut.Initialize();
      sut.GetDescriptorByMessageTypeName<CreateJusticeCase>(typeof(CreateJusticeCase).FullName!).Should()
        .NotBeNull("descriptor is present").And
        .BeAssignableTo<IIngressMessageTypeDescriptor<CreateJusticeCase>>("descriptor should support typed contract");
    }

    [Fact]
    public void when_descriptor_given_it_should_serialize_and_parse_message()
    {
      var ingressRegistry = new CaseOfficeIngressApiMessageTypesRegistry();
      ingressRegistry.Initialize();
      var ingressDescriptor = ingressRegistry.GetDescriptorByMessageTypeName<CreateJusticeCase>(typeof(CreateJusticeCase).FullName!);

      var egressRegistry = new CaseOfficeEgressApiMessageTypesRegistry();
      egressRegistry.Initialize();
      var egressDescriptor = egressRegistry.GetDescriptorByMessageType<CreateJusticeCase>();

      var message = new CreateJusticeCase {SubjectId = Guid.NewGuid(), Reason = "some reason", ResponsibleId = Guid.NewGuid()};
      using var bufferOwner = MemoryPool<byte>.Shared.Rent();
      var buffer = bufferOwner.Memory;

      egressDescriptor.Serialize(message, buffer);
      var parsedMessage = ingressDescriptor.Parse(buffer);
      parsedMessage.Should().NotBeSameAs(message, "parsed message should not be the same object as the original");
      parsedMessage.Should().BeEquivalentTo(message, "parsed message should have same property values as the original");
    }

    [Fact]
    public void when_registry_not_initialized_and_clr_type_for_known_message_type_requested_it_should_fail()
    {
      var registry = new CaseOfficeIngressApiMessageTypesRegistry();
      Action sut = () => registry.GetMessageTypeByItsMessageTypeName(typeof(CreateJusticeCase).FullName!);
      sut.Should().Throw<PoezdOperationException>("without initialization registry can not serve");
    }

    [Fact]
    public void when_registry_not_initialized_and_descriptor_for_known_message_type_requested_it_should_fail()
    {
      var registry = new CaseOfficeIngressApiMessageTypesRegistry();
      Action sut = () => registry.GetDescriptorByMessageTypeName<CreateJusticeCase>(typeof(CreateJusticeCase).FullName!);
      sut.Should().Throw<PoezdOperationException>("without initialization registry can not serve");
    }
  }
}
