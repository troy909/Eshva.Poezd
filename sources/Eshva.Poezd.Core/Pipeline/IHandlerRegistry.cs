#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Registry of message handlers specific for an API.
  /// </summary>
  public interface IHandlerRegistry
  {
    /// <summary>
    /// Gets message handler types grouped by handled message type.
    /// </summary>
    [NotNull]
    IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; }
  }
}
