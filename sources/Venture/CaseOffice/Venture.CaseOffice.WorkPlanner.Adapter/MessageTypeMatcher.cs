#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Venture.WorkPlanner.Messages;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  /// <summary>
  /// Message type full name to its type object matcher.
  /// </summary>
  internal class MessageTypeMatcher
  {
    public MessageTypeMatcher()
    {
      var messagesAssembly = Assembly.GetAssembly(typeof(AssemblyTag));
      var messageTypes = messagesAssembly!.ExportedTypes.Where(type => type.Namespace!.StartsWith("Venture.WorkPlanner.Messages.V1"));
      _types = messageTypes.ToDictionary(type => type.FullName, type => type);
    }

    /// <summary>
    /// Matches message type full name to the message type object.
    /// </summary>
    /// <param name="messageTypeName">
    /// Message type full name.
    /// </param>
    /// <returns>
    /// The found message type object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The message type full name is null, an empty or whitespace string.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Message type isn't found in the message assemblies.
    /// </exception>
    public Type MatchToType([NotNull] string messageTypeName)
    {
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));

      if (!_types.TryGetValue(messageTypeName, out var foundType))
        throw new InvalidOperationException($"Message type {messageTypeName} isn't found in the message assemblies.");
      return foundType;
    }

    private readonly Dictionary<string, Type> _types;
  }
}
