#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// An empty handler registry.
  /// </summary>
  internal class EmptyHandlerRegistry : IHandlerRegistry
  {
    /// <summary>
    /// Creates a new instance of handler registry.
    /// </summary>
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
