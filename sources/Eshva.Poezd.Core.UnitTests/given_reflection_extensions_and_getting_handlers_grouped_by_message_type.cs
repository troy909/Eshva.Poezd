#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Common;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class given_reflection_extensions_and_getting_handlers_grouped_by_message_type
  {
    [Fact]
    public void when_base_message_handlers_interface_provided_it_should_return_handlers_including_derived_message_handlers_interfaces()
    {
      var assemblies = new[] {typeof(H11).Assembly};
      var handlersGroupedByMessageType = assemblies.GetHandlersGroupedByMessageType(typeof(IHandler<>));

      handlersGroupedByMessageType.Count.Should().Be(expected: 3, "there is exactly 3 handled message types");

      handlersGroupedByMessageType[typeof(M1)].Should().BeEquivalentTo(
        new[]
        {
          typeof(H11),
          typeof(H12),
          typeof(H13)
        },
        $"these 3 handle {nameof(M1)}");
      handlersGroupedByMessageType[typeof(M2)].Should().BeEquivalentTo(
        new[]
        {
          typeof(H13),
          typeof(H21),
          typeof(H22)
        },
        $"these 3 handle {nameof(M2)}");
      handlersGroupedByMessageType[typeof(M3)].Should().BeEquivalentTo(
        new[]
        {
          typeof(H22)
        },
        $"this one handle {nameof(M3)}");
    }

    [Fact]
    public void when_derived_message_handlers_interface_provided_it_should_return_only_handlers_implementing_derived_interface()
    {
      var assemblies = new[] {typeof(H11).Assembly};
      var handlersGroupedByMessageType = assemblies.GetHandlersGroupedByMessageType(typeof(ISagaHandler<>));

      handlersGroupedByMessageType.Count.Should().Be(expected: 1, "there is exactly 1 handled message types");
      handlersGroupedByMessageType[typeof(M2)].Should().BeEquivalentTo(new[] {typeof(H13)}, "this one handle ISagaHandler<M2>}");
    }

    [Fact]
    public void when_message_handler_interface_not_provided_it_should_throw()
    {
      var assemblies = new[] { typeof(H11).Assembly };
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      Action sut = () => assemblies.GetHandlersGroupedByMessageType(null);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageHandlerInterface"),
        "message handler interface type should be specified");
    }

    [Fact]
    public void when_message_handler_interface_is_not_interface_type_it_should_throw()
    {
      var assemblies = new[] { typeof(H11).Assembly };
      Action sut = () => assemblies.GetHandlersGroupedByMessageType(typeof(H11));
      sut.Should().ThrowExactly<ArgumentException>().Where(
        exception => exception.ParamName.Equals("messageHandlerInterface"),
        "message handler interface type should be specified");
    }

    // ReSharper disable once UnusedTypeParameter
    private interface IHandler<TMessage> { }

    private interface ISagaHandler<TMessage> : IHandler<TMessage> { }

    private interface IAdditionalContract { }

    private class H11 : IHandler<M1> { }

    private class H12 : IHandler<M1> { }

    private class H13 : IHandler<M1>, ISagaHandler<M2> { }

    private class H21 : IHandler<M2>, IAdditionalContract { }

    private class H22 : IHandler<M2>, IHandler<M3>, IAdditionalContract { }


    private class M1 { }

    private class M2 { }

    private class M3 { }
  }
}
