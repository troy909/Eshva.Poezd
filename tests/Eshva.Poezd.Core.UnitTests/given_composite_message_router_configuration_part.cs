#region Usings

using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Configuration;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_composite_message_router_configuration_part
  {
    [Fact]
    public void when_validate_it_should_validate_itself()
    {
      var sut = new TestConfigurationPart();
      sut.Validate();
      sut.IsValidateItselfCalled.Should().BeTrue();
    }

    [Fact]
    public void when_validate_it_should_validate_children()
    {
      var sut = new TestConfigurationPart();
      sut.Validate();
      sut.IsGetChildConfigurationsCalled.Should().BeTrue();
    }

    [Fact]
    public void when_validate_it_should_collect_self_errors_and_errors_from_children()
    {
      var sut = new TestConfigurationPart();
      var errors = sut.Validate();
      errors.Should().BeEquivalentTo(
        TestConfigurationPart.Error,
        ChildConfiguration1.Error,
        ChildConfiguration2.Error);
    }

    [Fact]
    public void when_validate_and_some_configuration_part_not_specified_it_should_fail()
    {
      var sut = new ErroneousConfigurationPart();
      var errors = sut.Validate().ToArray();
      errors.Should().Contain(ChildConfiguration1.Error);
      errors.Should().Contain(ChildConfiguration2.Error);
      errors.Should().Contain(error => error.Contains("child properties"));
    }

    private class TestConfigurationPart : CompositeMessageRouterConfigurationPart
    {
      public bool IsValidateItselfCalled { get; private set; }

      public bool IsGetChildConfigurationsCalled { get; private set; }

      protected override IEnumerable<string> ValidateItself()
      {
        IsValidateItselfCalled = true;
        return new[] {Error};
      }

      protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations()
      {
        IsGetChildConfigurationsCalled = true;
        return new IMessageRouterConfigurationPart[] {new ChildConfiguration1(), new ChildConfiguration2()};
      }

      public const string Error = "self error";
    }

    private class ChildConfiguration1 : IMessageRouterConfigurationPart
    {
      public IEnumerable<string> Validate() => new[] {Error};

      public const string Error = "child1 error";
    }

    private class ChildConfiguration2 : IMessageRouterConfigurationPart
    {
      public IEnumerable<string> Validate() => new[] {Error};

      public const string Error = "child2 error";
    }

    private class ErroneousConfigurationPart : CompositeMessageRouterConfigurationPart
    {
      protected override IEnumerable<string> ValidateItself() => Enumerable.Empty<string>();

      protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations() =>
        new IMessageRouterConfigurationPart[] {new ChildConfiguration1(), null, new ChildConfiguration2()};
    }
  }
}
