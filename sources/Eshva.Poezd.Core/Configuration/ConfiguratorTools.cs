#region Usings

using System;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  internal static class ConfiguratorTools
  {
    public static PoezdConfigurationException MakeConfigurationMethodCalledMoreThanOnceException(
      [NotNull] string propertyDescription,
      string targetOfConfiguration,
      string configurationMethodName)
    {
      if (string.IsNullOrWhiteSpace(propertyDescription)) throw new ArgumentNullException(nameof(propertyDescription));
      if (string.IsNullOrWhiteSpace(targetOfConfiguration)) throw new ArgumentNullException(nameof(targetOfConfiguration));
      if (string.IsNullOrWhiteSpace(configurationMethodName)) throw new ArgumentNullException(nameof(configurationMethodName));

      return new PoezdConfigurationException(
        $"It's not allowed to set {propertyDescription} on {targetOfConfiguration} more than once.{Environment.NewLine}" +
        $"Check your message router configuration and unsure you call {configurationMethodName}() once per broker egress");
    }
  }
}
