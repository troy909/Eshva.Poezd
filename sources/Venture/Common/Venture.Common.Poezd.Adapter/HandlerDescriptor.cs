#region Usings

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter
{
  public readonly struct HandlerDescriptor
  {
    private static readonly Func<object, VentureContext, Task> Nope = (message, context) => Task.CompletedTask;

    public HandlerDescriptor(
      [NotNull] Type type,
      [NotNull] Func<object, VentureContext, Task> onHandle)
    {
      Type = type ?? throw new ArgumentNullException(nameof(type));
      OnHandle = onHandle ?? throw new ArgumentNullException(nameof(onHandle));
    }

    public Type Type { get; }

    public Func<object, VentureContext, Task> OnHandle { get; }
  }
}
