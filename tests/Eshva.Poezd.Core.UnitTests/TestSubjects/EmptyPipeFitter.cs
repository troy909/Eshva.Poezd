#region Usings

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Pipe fitter producing no steps.
  /// </summary>
  [ExcludeFromCodeCoverage]
  internal class EmptyPipeFitter : IPipeFitter
  {
    /// <inheritdoc />
    public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class { }
  }
}
