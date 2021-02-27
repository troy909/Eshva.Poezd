#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// A step of message processing pipeline.
  /// </summary>
  public interface IStep<in TContext> where TContext : class
  {
    /// <summary>
    /// Executes this step.
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of message processing context.
    /// </typeparam>
    /// <param name="context">
    /// Message handling context containing items required for handling a broker message.
    /// </param>
    /// <returns>
    /// A task that can be used for waiting the step is completed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The message handling context isn't provided.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// A required context item isn't found.
    /// </exception>
    /// <exception cref="PoezdOperationException">
    /// An error occurred during step execution.
    /// </exception>
    [NotNull]
    Task Execute([NotNull] TContext context);
  }
}
