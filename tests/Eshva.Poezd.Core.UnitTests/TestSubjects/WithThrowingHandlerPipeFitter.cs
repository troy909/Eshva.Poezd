#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  // ReSharper disable once ClassNeverInstantiated.Global
  public class WithThrowingHandlerPipeFitter : TypeBasedPipeFitter
  {
    public WithThrowingHandlerPipeFitter([NotNull] IDiContainerAdapter serviceProvider) : base(serviceProvider) { }

    protected override IEnumerable<Type> GetStepTypes()
    {
      yield return typeof(ThrowingIngressStep);
    }
  }
}
