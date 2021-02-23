#region Usings

using System;
using System.Linq;
using Eshva.Poezd.Core.Common;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_reflection_extensions_getting_handlers_grouped_by_message_type : IClassFixture<GetHandlersByGroupClassFixture>
  {
    public given_reflection_extensions_getting_handlers_grouped_by_message_type(GetHandlersByGroupClassFixture classFixture)
    {
      _classFixture = classFixture;
    }

    [Fact]
    public void when_assembly_contains_handlers_it_should_return_grouped_handlers()
    {
      var handler1Interface = _classFixture.InterfaceAssembly.GetType("TestSubjects.IHandler1`1");
      var assemblies = new[] {_classFixture.Handlers1Assembly, _classFixture.Handlers2Assembly, _classFixture.Handlers3Assembly};

      var handler1Types = assemblies.GetHandlersGroupedByMessageType(handler1Interface!);

      handler1Types.Single(pair => pair.Key.FullName!.Equals("TestSubjects.Message1"))
        .Should().NotBeNull("there is a handler in assembly");
      handler1Types.Single(pair => pair.Key.FullName!.Equals("TestSubjects.Message1")).Value
        .Should().HaveCount(expected: 4, "there is 4 handlers of Message1 in 3 handler assemblies");
    }

    [Fact]
    public void when_assembly_contains_no_handlers_it_should_return_empty_dictionary()
    {
      var handlerInterface = _classFixture.InterfaceAssembly.GetType("TestSubjects.IHandler2`1");
      var assemblies = new[] {_classFixture.Handlers1Assembly};

      var messageTypes = assemblies.GetHandlersGroupedByMessageType(handlerInterface!);
      messageTypes.Should().NotBeNull("even if handlers not found null never should be returned");
      messageTypes.Should().HaveCount(expected: 0, "there is no handlers of this type");
    }

    [Fact]
    public void when_handler_interface_not_specified_it_should_fail()
    {
      var assemblies = new[] {_classFixture.Handlers1Assembly};

      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => assemblies.GetHandlersGroupedByMessageType(messageHandlerInterface: null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("messageHandlerInterface"));
    }

    [Fact]
    public void when_handler_interface_is_not_interface_it_should_fail()
    {
      var handlerInterface = _classFixture.InterfaceAssembly.GetType("TestSubjects.Message1");
      var assemblies = new[] {_classFixture.Handlers1Assembly};

      Action sut = () => assemblies.GetHandlersGroupedByMessageType(handlerInterface!);
      sut.Should().ThrowExactly<ArgumentException>("a class type is not an interface type");
    }

    [Fact]
    public void when_handler_interface_has_no_generic_argument_it_should_fail()
    {
      var handlerInterface = _classFixture.InterfaceAssembly.GetType("TestSubjects.IAmNotGood0");
      var assemblies = new[] {_classFixture.Handlers1Assembly};

      Action sut = () => assemblies.GetHandlersGroupedByMessageType(handlerInterface!);
      sut.Should().ThrowExactly<ArgumentException>("handlers interface can not have no type arguments");
    }

    [Fact]
    public void when_handler_interface_has_more_than_one_generic_argument_it_should_fail()
    {
      var handlerInterface = _classFixture.InterfaceAssembly.GetType("TestSubjects.IAmNotGood2`2");
      var assemblies = new[] {_classFixture.Handlers1Assembly};

      Action sut = () => assemblies.GetHandlersGroupedByMessageType(handlerInterface!);
      sut.Should().ThrowExactly<ArgumentException>("handlers interface can not have more than 1 type arguments");
    }

    private readonly GetHandlersByGroupClassFixture _classFixture;
  }
}
