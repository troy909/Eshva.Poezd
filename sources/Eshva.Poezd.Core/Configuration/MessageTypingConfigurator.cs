using System;


namespace Eshva.Poezd.Core.Configuration
{
  public class MessageTypingConfigurator
  {
    public IMessageTypingConvention Convention { get; set; }

    public MessageTypingConfigurator UseConvention(IMessageTypingConvention convention)
    {
      Convention = convention ?? throw new ArgumentNullException(nameof(convention));
      return this;
    }
  }
}