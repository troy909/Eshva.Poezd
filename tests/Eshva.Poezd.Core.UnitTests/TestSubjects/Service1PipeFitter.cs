#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  [UsedImplicitly]
  public sealed class Service1PipeFitter : TypeBasedPipeFitter
  {
    public Service1PipeFitter([NotNull] IDiContainerAdapter serviceProvider) : base(serviceProvider) { }

    protected override IEnumerable<Type> GetStepTypes()
    {
      yield return typeof(Service1DeserializeMessageStep);
      yield return typeof(GetMessageHandlersStep);
      yield return typeof(DispatchMessageToHandlersStep);
      yield return typeof(CommitMessageStep);
    }
  }
}
