#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.IntegrationTests.TestSubjects
{
  public class EmptyHandlerRegistry : IHandlerRegistry
  {
    public EmptyHandlerRegistry()
    {
      HandlersGroupedByMessageType = new Dictionary<Type, Type[]>
      {
        {typeof(string), new[] {typeof(string)}}
      };
    }

    public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; }
  }
}
