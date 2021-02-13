#region Usings

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  public readonly struct HandlerDescriptor
  {
    private static readonly Func<object, VentureContext, Task> Nope = (message, context) => Task.CompletedTask;

    public HandlerDescriptor(
      [NotNull] Type type,
      [NotNull] Func<object, VentureContext, Task> onHandle,
      [CanBeNull] Func<object, VentureContext, Task> onCommit = null,
      [CanBeNull] Func<object, VentureContext, Task> onCompensate = null)
    {
      Type = type ?? throw new ArgumentNullException(nameof(type));
      OnHandle = onHandle ?? throw new ArgumentNullException(nameof(onHandle));
      OnCommit = onCommit ?? Nope;
      OnCompensate = onCompensate ?? Nope;
    }

    public Type Type { get; }

    public Func<object, VentureContext, Task> OnHandle { get; }

    public Func<object, VentureContext, Task> OnCommit { get; }

    public Func<object, VentureContext, Task> OnCompensate { get; }
  }
}
