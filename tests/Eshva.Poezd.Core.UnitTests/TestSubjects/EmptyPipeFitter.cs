#region Usings

using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
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
