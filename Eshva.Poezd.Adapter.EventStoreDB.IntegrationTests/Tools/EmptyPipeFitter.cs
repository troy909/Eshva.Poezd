#region Usings

using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.IntegrationTests.Tools
{
  /// <summary>
  /// Pipe fitter producing no steps.
  /// </summary>
  [ExcludeFromCodeCoverage]
  [UsedImplicitly]
  internal class EmptyPipeFitter : IPipeFitter
  {
    /// <inheritdoc />
    public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class { }
  }
}
