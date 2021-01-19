#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  /// <summary>
  /// Service #1 is the point of view of the imaginary sample application. It should handle all commands for service #1
  /// and public events of itself.
  /// It should handle them using message handlers in the application layer of service #1.
  /// TODO: Add handling of process managers and sagas.
  /// </summary>
  [UsedImplicitly]
  public sealed class Service1Adapter : IPublicApiAdapter
  {
    public IEnumerable<IStep> GetPipelineSteps() =>
      new IStep[]
      {
        // new Service1DeserializeMessageStep(),
        // new GetMessageHandlersStep(),
        // new DispatchMessageToHandlersStep(),
        // new CommitMessageStep()
      };
  }
}
