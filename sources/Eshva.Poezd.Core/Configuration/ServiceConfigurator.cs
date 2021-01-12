using System;


namespace Eshva.Poezd.Core.Configuration
{
  public class ServiceConfigurator
  {
    public string Name { get; private set; }

    public ServiceConfigurator WithName(string name)
    {
      Name = name;
      return this;
    }

    public ServiceConfigurator WithSerialization(Action<SerializationConfigurator> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      configurator(new SerializationConfigurator());
      return this;
    }

    public ServiceConfigurator WithMetadataHandling(Action<MetadataHandlingConfigurator> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      configurator(new MetadataHandlingConfigurator());
      return this;
    }

    public ServiceConfigurator WithMessageTypingConvention(Action<MessageTypingConfigurator> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      configurator(new MessageTypingConfigurator());
      return this;
    }

    public ServiceConfigurator WithQueueNamingConvention(Action<QueueNamingConfigurator> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      configurator(new QueueNamingConfigurator());

      return this;
    }
  }
}