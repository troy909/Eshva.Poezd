#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.IntegrationTests.Tools
{
  /// <summary>
  /// An empty handler registry.
  /// </summary>
  [ExcludeFromCodeCoverage]
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
