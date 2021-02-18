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
      var sut = new CaseOfficeMessageTypesRegistry();
      sut.Initialize();
      sut.GetType(typeof(CreateCase).FullName!).Should().NotBeNull("type is known");
    }

    [Fact]
    public void when_registry_initialized_and_descriptor_for_known_message_type_requested_it_should_return_descriptor()
    {
      var sut = new CaseOfficeMessageTypesRegistry();
      sut.Initialize();
      sut.GetDescriptor<CreateCase>(typeof(CreateCase).FullName!).Should()
        .NotBeNull("descriptor is present").And
        .BeAssignableTo<IMessageTypeDescriptor<CreateCase>>("descriptor should support typed contract");
    }

    [Fact]
    public void when_descriptor_given_it_should_serialize_and_parse_message()
    {
      var registry = new CaseOfficeMessageTypesRegistry();
      registry.Initialize();
      var sut = registry.GetDescriptor<CreateCase>(typeof(CreateCase).FullName!);
      var message = new CreateCase {CaseId = Guid.NewGuid(), SubjectId = Guid.NewGuid(), CaseType = "case type", Reason = "some reason"};
      using var bufferOwner = MemoryPool<byte>.Shared.Rent();
      var buffer = bufferOwner.Memory;

      var bytesSerialized = sut.Serialize(message, buffer.Span);
      bytesSerialized.Should().BeGreaterThan(expected: 0, "something should be serialized into byte buffer");

      var parsedMessage = sut.Parse(buffer);
      parsedMessage.Should().NotBeSameAs(message, "parsed message should not be the same object as the original");
      parsedMessage.Should().BeEquivalentTo(message, "parsed message should have same property values as the original");
    }

    [Fact]
    public void when_registry_not_initialized_and_clr_type_for_known_message_type_requested_it_should_fail()
    {
      var registry = new CaseOfficeMessageTypesRegistry();
      Action sut = () => registry.GetType(typeof(CreateCase).FullName!);
      sut.Should().Throw<PoezdOperationException>("without initialization registry can not serve");
    }

    [Fact]
    public void when_registry_not_initialized_and_descriptor_for_known_message_type_requested_it_should_fail()
    {
      var registry = new CaseOfficeMessageTypesRegistry();
      Action sut = () => registry.GetDescriptor<CreateCase>(typeof(CreateCase).FullName!);
      sut.Should().Throw<PoezdOperationException>("without initialization registry can not serve");
    }
  }
}
