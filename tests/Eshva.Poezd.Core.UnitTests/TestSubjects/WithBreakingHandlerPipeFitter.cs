#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class WithBreakingHandlerPipeFitter : TypeBasedPipeFitter
  {
    public WithBreakingHandlerPipeFitter([NotNull] IDiContainerAdapter serviceProvider) : base(serviceProvider) { }

    protected override IEnumerable<Type> GetStepTypes()
    {
      yield return typeof(BreakingIngressStep);
      yield return typeof(LogMessageHandlingContextStep);
    }
  }
}
