#region Usings

using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.IntegrationTests.TestSubjects
{
  /// <summary>
  /// Pipe fitter producing no steps.
  /// </summary>
  internal class EmptyPipeFitter : IPipeFitter
  {
    /// <inheritdoc />
    public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class { }
  }
}
