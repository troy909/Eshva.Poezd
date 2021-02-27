#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter.MessageHandling
{
  public class VentureServiceHandlersRegistry : IHandlerRegistry
  {
    public VentureServiceHandlersRegistry(IEnumerable<Assembly> messageHandlersAssemblies)
    {
      HandlersGroupedByMessageType = messageHandlersAssemblies
        .GetHandlersGroupedByMessageType(typeof(IMessageHandler<>))
        .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; }
  }
}
