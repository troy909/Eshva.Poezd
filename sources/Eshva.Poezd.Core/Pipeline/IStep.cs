#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// A step of broker message handling pipeline.
  /// </summary>
  public interface IStep
  {
    /// <summary>
    /// Executes this step.
    /// </summary>
    /// <param name="context">
    /// Message handling context containing items required for handling a broker message.
    /// </param>
    /// <returns>
    /// A task that can be used for waiting the step is completed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The message handling context isn't provided.
    /// </exception>
    Task Execute([NotNull] IPocket context);
  }
}
