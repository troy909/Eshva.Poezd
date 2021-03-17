#region Usings

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Venture.Common.Application.Ingress;

#endregion

namespace Venture.Common.Poezd.Adapter
{
  public readonly struct HandlerDescriptor
  {
    public HandlerDescriptor(
      [NotNull] Type type,
      [NotNull] Type messageType,
      [NotNull] Func<object, VentureIncomingMessageHandlingContext, Task> onHandle)
    {
      HandlerType = type ?? throw new ArgumentNullException(nameof(type));
      MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));
      OnHandle = onHandle ?? throw new ArgumentNullException(nameof(onHandle));
    }

    public Type HandlerType { get; }

    public Type MessageType { get; }

    public Func<object, VentureIncomingMessageHandlingContext, Task> OnHandle { get; }
  }
}
