#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eshva.Poezd.Core.Common;
using FlatSharp;
using JetBrains.Annotations;
using Venture.WorkPlanner.Messages;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  /// <summary>
  /// Message type registry.
  /// </summary>
  internal sealed class MessageTypeRegistry
  {
    public MessageTypeRegistry()
    {
      var messagesAssembly = Assembly.GetAssembly(typeof(AssemblyTag));
      var messageTypes = messagesAssembly!.ExportedTypes.Where(type => type.Namespace!.StartsWith("Venture.WorkPlanner.Messages.V1"));

      _descriptors ??= CreateDescriptors(messageTypes);
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
    public Type GetType([NotNull] string messageTypeName) => GetMessageTypeDescriptor(messageTypeName).Type;

    /// <summary>
    /// Parses message serialized as a byte array using FlatBuffers.
    /// </summary>
    /// <param name="messageTypeName">
    /// Message type full name.
    /// </param>
    /// <param name="bytes">
    /// Serialized message bytes.
    /// </param>
    /// <returns>
    /// Deserialized message object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The message type full name is null, an empty or whitespace string.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Message type isn't found in the message assemblies.
    /// </exception>
    public object Parse([NotNull] string messageTypeName, [NotNull] byte[] bytes)
    {
      if (bytes == null) throw new ArgumentNullException(nameof(bytes));
      return GetMessageTypeDescriptor(messageTypeName).Parse(bytes);
    }

    private static Dictionary<string, MessageTypeDescriptor> CreateDescriptors(IEnumerable<Type> messageTypes)
    {
      return messageTypes.ToDictionary(
        type => type.FullName,
        type => new MessageTypeDescriptor(
          type,
          bytes =>
          {
            try
            {
              return GetParseMethod(type).Invoke(Serializer, new object[] {bytes});
            }
            catch (Exception exception)
            {
              throw new PoezdOperationException($"Unable to deserialize message with type {type.FullName}.", exception);
            }
          }));
    }

    private static MessageTypeDescriptor GetMessageTypeDescriptor(string messageTypeName)
    {
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));

      if (!_descriptors.TryGetValue(messageTypeName, out var descriptor))
        throw new InvalidOperationException($"Message type {messageTypeName} isn't found in the message assemblies.");

      return descriptor;
    }

    private static MethodInfo GetParseMethod(Type messageType)
    {
      var openGeneric = SerializerType
        .GetMethods()
        .Single(method => method.Name == ParseName && method.GetParameters().Any(parameter => parameter.ParameterType == typeof(byte[])));
      return openGeneric.MakeGenericMethod(messageType);
    }

    private const string ParseName = nameof(FlatBufferSerializer.Parse);
    private static readonly FlatBufferSerializer Serializer = new FlatBufferSerializer();
    private static readonly Type SerializerType = typeof(FlatBufferSerializer);
    private static Dictionary<string, MessageTypeDescriptor> _descriptors;

    private readonly struct MessageTypeDescriptor
    {
      public MessageTypeDescriptor(Type type, Func<byte[], object> parse)
      {
        Parse = parse;
        Type = type;
      }

      public readonly Type Type;
      public readonly Func<byte[], object> Parse;
    }
  }
}
