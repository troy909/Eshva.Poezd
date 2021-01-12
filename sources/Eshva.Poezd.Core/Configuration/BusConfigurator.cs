using System;
using JetBrains.Annotations;


namespace Eshva.Poezd.Core.Configuration
{
  public class BusConfigurator
  {
    public string Name { get; private set; }

    public BusConfigurator WithName(string name)
    {
      Name = name;
      return this;
    }

    public BusConfigurator WithMessageBroker([NotNull] Action<MessageBrokerConfigurator> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      configurator(new MessageBrokerConfigurator());
      return this;
    }

    public BusConfigurator AddService(Action<ServiceConfigurator> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      configurator(new ServiceConfigurator());
      return this;
    }
  }
}