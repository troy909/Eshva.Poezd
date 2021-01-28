#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Venture.IntegrationTests.TestSubjects
{
  public class MessageCountingPipelineConfigurator : TypeBasedLinearPipelineConfigurator
  {
    public MessageCountingPipelineConfigurator([NotNull] IServiceProvider serviceProvider) : base(serviceProvider) { }

    protected override IEnumerable<Type> GetStepTypes()
    {
      yield return typeof(CounterStep);
    }
  }
}
