#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Eshva.Poezd.Core.Common;
using FlatSharp;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.Poezd.Adapter
{
  /// <summary>
  /// Message type registry.
  /// </summary>
  public sealed class MessageTypesRegistry
  {
    public MessageTypesRegistry([NotNull] IEnumerable<Type> messageTypes)
    {
      if (messageTypes == null) throw new ArgumentNullException(nameof(messageTypes));

      _descriptors = CreateDescriptors(messageTypes);
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
    public Type GetType([NotNull] string messageTypeName)
    {
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));
      return GetMessageTypeDescriptor(messageTypeName).Type;
    }

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
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));
      if (bytes == null) throw new ArgumentNullException(nameof(bytes));

      return GetMessageTypeDescriptor(messageTypeName).Parse(bytes);
    }

    public byte[] Serialize([NotNull] string messageTypeName, [NotNull] object message)
    {
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));
      if (message == null) throw new ArgumentNullException(nameof(message));

      return GetMessageTypeDescriptor(messageTypeName).Serialize(message);
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
              throw new PoezdOperationException($"Unable to deserialize a message of type {type.FullName}.", exception);
            }
          },
          message =>
          {
            try
            {
              var bufferSize = (int) GetGetMaxSizeMethod(type).Invoke(Serializer, new[] {message});
              var buffer = new byte[bufferSize];
              GetSerializeMethod(type).Invoke(Serializer, new[] {message, buffer});
              return buffer;
            }
            catch (Exception exception)
            {
              throw new PoezdOperationException($"Unable to serialize a message of type {type.FullName}.", exception);
            }
          })
      );
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
        .Single(
          method => method.Name == ParseMethodName &&
                    method.GetParameters().Any(parameter => parameter.ParameterType == typeof(byte[])));
      return openGeneric.MakeGenericMethod(messageType);
    }

    private static MethodInfo GetSerializeMethod(Type messageType)
    {
      var openGeneric = SerializerType
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .Single(
          method => method.Name == SerializeMethodName &&
                    method.GetParameters().Length == 2);
      return openGeneric.MakeGenericMethod(messageType);
    }

    private static MethodInfo GetGetMaxSizeMethod(Type messageType)
    {
      var openGeneric = SerializerType
        .GetMethods()
        .Single(
          method => method.Name == GetMaxSizeMethodName &&
                    method.GetParameters().Length == 1);
      return openGeneric.MakeGenericMethod(messageType);
    }

    private const string ParseMethodName = nameof(FlatBufferSerializer.Parse);
    private const string SerializeMethodName = "SerializeInternal";
    private const string GetMaxSizeMethodName = nameof(FlatBufferSerializer.GetMaxSize);
    private static readonly FlatBufferSerializer Serializer = new FlatBufferSerializer();
    private static readonly Type SerializerType = typeof(FlatBufferSerializer);
    private static Dictionary<string, MessageTypeDescriptor> _descriptors;

    private readonly struct MessageTypeDescriptor
    {
      public MessageTypeDescriptor(
        Type type,
        Func<byte[], object> parse,
        Func<object, byte[]> serialize)
      {
        Type = type;
        Parse = parse;
        Serialize = serialize;
      }

      public readonly Type Type;
      public readonly Func<byte[], object> Parse;
      public readonly Func<object, byte[]> Serialize;
    }
  }
}
