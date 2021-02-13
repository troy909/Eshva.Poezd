#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter
{
  public class VentureServiceHandlersRegistry : IHandlerRegistry
  {
    public VentureServiceHandlersRegistry(IEnumerable<Assembly> messageHandlersAssemblies, string messagesNamespace)
    {
      HandlersGroupedByMessageType = messageHandlersAssemblies
        .GetHandlersGroupedByMessageType(typeof(IHandleMessageOfType<>))
        .Where(pair => pair.Key.FullName!.StartsWith(messagesNamespace))
        .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; }
  }
}
