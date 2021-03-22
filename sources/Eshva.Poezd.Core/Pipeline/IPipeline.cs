#region Usings

using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Contract of message processing pipeline.
  /// </summary>
  [UsedImplicitly(ImplicitUseTargetFlags.Members)]
  public interface IPipeline<TContext> where TContext : class
  {
    /// <summary>
    /// Appends the <paramref name="step" /> at end of the pipeline.
    /// </summary>
    /// <param name="step">
    /// The step to append.
    /// </param>
    /// <returns>
    /// This pipeline.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// The context is not specified.
    /// </exception>
    [NotNull]
    IPipeline<TContext> Append([NotNull] IStep<TContext> step);

    /// <summary>
    /// Executes pipeline for the message which is included into the <paramref name="context" />.
    /// </summary>
    /// <param name="context">
    /// The message handling context.
    /// </param>
    /// <returns>
    /// A task that can be used to wait message handling is finished.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// The context is not specified.
    /// </exception>
    /// <exception cref="PoezdOperationException">
    /// Some error occurred during message handling. Inspect the inner exception for details.
    /// </exception>
    [NotNull]
    Task Execute([NotNull] TContext context);
  }
}
