#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Activation;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class MessageHandlingConfiguration : IMessageRouterConfigurationPart
  {
    public IMessageHandlersFactory MessageHandlersFactory { get; set; }

    public IEnumerable<string> Validate()
    {
      yield break;
    }
  }
}
