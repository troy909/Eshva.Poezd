#region Usings

using System;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public class PoezdConfiguration
  {
    private PoezdConfiguration()
    {
    }

    public MessageHandling MessageHandling { get; } = new MessageHandling();

    public static PoezdConfiguration Create([NotNull] Action<PoezdConfiguration> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      var configuration = new PoezdConfiguration();
      configurator(configuration);
      return configuration;
    }

    public PoezdConfiguration WithMessageHandling(Action<MessageHandlingConfigurator> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      configurator(new MessageHandlingConfigurator(MessageHandling));
      return this;
    }
  }
}
