using System;


namespace Eshva.Poezd.Core.Configuration
{
  public class SerializationConfigurator
  {
    public IMessageSerializer MessageSerializer { get; private set; }

    public SerializationConfigurator UseSerializer(IMessageSerializer messageSerializer)
    {
      MessageSerializer = messageSerializer ?? throw new ArgumentNullException(nameof(messageSerializer));
      return this;
    }
  }
}