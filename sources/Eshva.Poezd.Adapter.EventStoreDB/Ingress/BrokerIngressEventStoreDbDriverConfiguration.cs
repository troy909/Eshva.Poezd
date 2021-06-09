#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.Ingress
{
  public class BrokerIngressEventStoreDbDriverConfiguration : IMessageRouterConfigurationPart
  {
    public EventStoreDbConnectionConfiguration ConnectionConfiguration { get; internal set; }

    public Type HeaderValueCodecType { get; internal set; }

    public Type StreamSubscriptionFactoryType { get; internal set; }

    // TODO: Implement configuration validation.
    public IEnumerable<string> Validate() => Enumerable.Empty<string>();
  }
}
