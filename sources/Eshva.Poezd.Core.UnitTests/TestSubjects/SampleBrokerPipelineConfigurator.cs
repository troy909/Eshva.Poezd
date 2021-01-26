#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  [UsedImplicitly]
  public class SampleBrokerPipelineConfigurator : TypeBasedLinearPipelineConfigurator
  {
    public SampleBrokerPipelineConfigurator([NotNull] IServiceProvider serviceProvider) : base(serviceProvider) { }

    protected override IEnumerable<Type> GetStepTypes()
    {
      yield return typeof(LogMessageHandlingContextStep);
    }
  }
}
