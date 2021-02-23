#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// A stab for handler registry.
  /// </summary>
  public class EmptyHandlerRegistry : IHandlerRegistry
  {
    public EmptyHandlerRegistry()
    {
      HandlersGroupedByMessageType = new Dictionary<Type, Type[]>
      {
        {typeof(string), new[] {typeof(string)}}
      };
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; }
  }
}
