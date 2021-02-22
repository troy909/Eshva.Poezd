#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Common
{
  /// <summary>
  /// Reflection extensions used by Poezd.
  /// </summary>
  public static class ReflectionExtensions
  {
    /// <summary>
    /// Groups message handlers of type <paramref name="messageHandlerInterface" /> from assemblies
    /// <paramref name="messageHandlersAssemblies" /> by CLR message type.
    /// </summary>
    /// <param name="messageHandlersAssemblies">
    /// Assemblies used to search message handlers in.
    /// </param>
    /// <param name="messageHandlerInterface">
    /// Type of interface used to mark message handlers in the application.
    /// </param>
    /// <returns>
    /// Read-only dictionary containing message handlers where key is message type and value an array of message handlers
    /// types.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Message handler interface is not specified.
    /// </exception>
    [NotNull]
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

      if (messageHandlerInterface.GetGenericArguments().Length != 1)
      {
        throw new ArgumentException("Message handler interface should have exactly 1 generic argument.");
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
