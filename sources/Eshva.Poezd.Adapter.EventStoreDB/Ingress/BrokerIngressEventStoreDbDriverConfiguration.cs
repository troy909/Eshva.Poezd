#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.Ingress
{
  public class BrokerIngressEventStoreDbDriverConfiguration : IMessageRouterConfigurationPart
  {
    public IEnumerable<string> Validate() => throw new NotImplementedException();
  }
}
