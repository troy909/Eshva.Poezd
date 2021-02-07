#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.IntegrationTests.TestSubjects
{
  public class PublicApi1HandlerRegistry : IHandlerRegistry
  {
    public PublicApi1HandlerRegistry()
    {
      HandlersGroupedByMessageType = new Dictionary<Type, Type[]>
      {
        {typeof(string), new[] {typeof(string)}}
      };
    }

    public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; }
  }
}
