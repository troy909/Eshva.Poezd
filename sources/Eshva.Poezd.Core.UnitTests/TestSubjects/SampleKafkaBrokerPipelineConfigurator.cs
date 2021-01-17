#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  [UsedImplicitly]
  public class SampleKafkaBrokerPipelineConfigurator : IPipelineConfigurator
  {
    /// <remarks>
    /// TODO: Add message audit logging or some other very general actions.
    /// </remarks>>
    public IPipeline ConfigurePipeline([NotNull] IPipeline pipeline) => pipeline ?? throw new ArgumentNullException(nameof(pipeline));
  }
}
