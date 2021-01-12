using System;


namespace Eshva.Poezd.Core.Configuration
{
  public class QueueNamingConfigurator
  {
    public IQueueNamingConvention Convention { get; private set; }

    public QueueNamingConfigurator UseConvention(IQueueNamingConvention convention)
    {
      Convention = convention ?? throw new ArgumentNullException(nameof(convention));
      return this;
    }
  }
}