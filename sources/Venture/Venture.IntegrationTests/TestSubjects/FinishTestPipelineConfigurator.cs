#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Venture.IntegrationTests.TestSubjects
{
  public class FinishTestPipelineConfigurator : TypeBasedLinearPipelineConfigurator
  {
    public FinishTestPipelineConfigurator([NotNull] IServiceProvider serviceProvider) : base(serviceProvider) { }

    protected override IEnumerable<Type> GetStepTypes()
    {
      yield return typeof(FinishTestStep);
    }
  }
}
