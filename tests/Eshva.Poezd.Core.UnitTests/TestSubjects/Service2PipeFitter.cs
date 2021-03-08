#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  /// <summary>
  /// In the imaginary sample application the service #2 is an external service developed by another team. The service #1
  /// should handle some of it's events. Message payload encoded as JSON-documents of known schema.
  /// </summary>
  [UsedImplicitly]
  public sealed class Service2PipeFitter : TypeBasedPipeFitter
  {
    public Service2PipeFitter([NotNull] IServiceProvider serviceProvider) : base(serviceProvider) { }

    protected override IEnumerable<Type> GetStepTypes()
    {
      yield return typeof(Service2DeserializeMessageStep);
      yield return typeof(GetMessageHandlersStep);
      yield return typeof(DispatchMessageToHandlersStep);
      yield return typeof(CommitMessageStep);
    }
  }
}
