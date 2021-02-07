#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Common
{
  public static class ReflectionExtensions
  {
    public static IReadOnlyDictionary<Type, Type[]> GetHandlersGroupedByMessageType(
      this IEnumerable<Assembly> messageHandlersAssemblies,
      [NotNull] Type messageHandlerInterface)
    {
      if (messageHandlerInterface == null) throw new ArgumentNullException(nameof(messageHandlerInterface));
      if (!messageHandlerInterface.IsInterface)
      {
        throw new ArgumentException(
          $"You should provide an interface type for {nameof(messageHandlerInterface)}.",
          nameof(messageHandlerInterface));
      }

      var allHandlerTypes = messageHandlersAssemblies.SelectMany(assembly => assembly.GetTypes())
        .Where(
          handlerType => handlerType.IsClass &&
                         handlerType.GetInterfaces().Any(type => type.IsHandlerInterface(messageHandlerInterface)))
        .ToArray();

      var handledMessageTypes = allHandlerTypes
        .SelectMany(
          handlerType => handlerType.GetInterfaces()
            .Where(type => type.IsHandlerInterface(messageHandlerInterface))
            .Select(type => type.GetGenericArguments().Single()))
        .Distinct();

      return handledMessageTypes.ToDictionary(
        messageType => messageType,
        messageType => allHandlerTypes.SelectMany(
            handlerType => handlerType.GetInterfaces()
              .Where(
                type => type.IsHandlerInterface(messageHandlerInterface) &&
                        type.GetGenericArguments().Single() == messageType)
              .Select(_ => handlerType))
          .ToArray());
    }

    private static bool IsHandlerInterface(this Type type, Type messageHandlerInterface) =>
      type.IsGenericType && type.GetGenericTypeDefinition() == messageHandlerInterface;
  }
}
