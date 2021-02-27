#region Usings

using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Contract of type that constructs pipeline.
  /// </summary>
  public interface IPipeFitter
  {
    /// <summary>
    /// Appends steps into <paramref name="pipeline" />.
    /// </summary>
    /// <param name="pipeline">
    /// Pipeline into which steps of this pipe fitter should be appended.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// Pipeline object is not specified.
    /// </exception>
    void AppendStepsInto<TContext>([NotNull] IPipeline<TContext> pipeline) where TContext : class;
  }
}
