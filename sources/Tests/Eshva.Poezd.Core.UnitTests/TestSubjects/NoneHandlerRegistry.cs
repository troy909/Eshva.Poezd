#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class NoneHandlerRegistry : IHandlerRegistry
  {
    public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; } = new Dictionary<Type, Type[]>();
  }
}
