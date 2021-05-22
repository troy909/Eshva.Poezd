#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Configuration;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_configurator_tools
  {
    [Fact]
    public void when_make_exception_of_configuration_method_called_more_than_once_it_should_produce_expected_exception_and_message()
    {
      const string propertyDescription = nameof(propertyDescription);
      const string targetOfConfiguration = nameof(targetOfConfiguration);
      const string configurationMethodName = nameof(configurationMethodName);
      var exception = ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
        propertyDescription,
        targetOfConfiguration,
        configurationMethodName);

      exception.Message.Should().Be(
        $"It's not allowed to set {propertyDescription} on {targetOfConfiguration} more than once.{Environment.NewLine}" +
        $"Check your message router configuration and unsure you call {configurationMethodName}() once per broker egress");
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_make_exception_of_configuration_method_called_more_than_once_with_wrong_arguments_it_should_fail()
    {
      var propertyDescription = "propertyDescription";
      var targetOfConfiguration = "targetOfConfiguration";
      var configurationMethodName = "configurationMethodName";
      Action sut = () => ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
        propertyDescription,
        targetOfConfiguration,
        configurationMethodName);

      propertyDescription = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("propertyDescription");
      propertyDescription = string.Empty;
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("propertyDescription");
      propertyDescription = WhitespaceString;
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("propertyDescription");
      propertyDescription = "propertyDescription";

      targetOfConfiguration = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("targetOfConfiguration");
      targetOfConfiguration = string.Empty;
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("targetOfConfiguration");
      targetOfConfiguration = WhitespaceString;
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("targetOfConfiguration");
      targetOfConfiguration = "targetOfConfiguration";

      configurationMethodName = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("configurationMethodName");
      configurationMethodName = string.Empty;
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("configurationMethodName");
      configurationMethodName = WhitespaceString;
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("configurationMethodName");
      configurationMethodName = "configurationMethodName";
    }

    private const string WhitespaceString = " \t\r\n";
  }
}
