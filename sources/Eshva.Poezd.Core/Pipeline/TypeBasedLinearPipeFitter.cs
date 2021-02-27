#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Pipe fitter which configuration based on specifying ordered list of step types.
  /// </summary>
  public abstract class TypeBasedLinearPipeFitter : IPipeFitter
  {
    /// <summary>
    /// Constructs an instance of pipe fitter.
    /// </summary>
    /// <param name="serviceProvider">
    /// Service provider used for constructing steps instances.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Service provider is not specified.
    /// </exception>
    protected TypeBasedLinearPipeFitter([NotNull] IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class
    {
      foreach (var stepType in GetStepTypes())
      {
        pipeline.Append((IStep<TContext>) _serviceProvider.GetService(stepType, MakeException));
      }
    }

    /// <summary>
    /// Returns an ordered list of step types.
    /// </summary>
    /// <returns>
    /// Ordered list of step types.
    /// </returns>
    [NotNull]
    protected abstract IEnumerable<Type> GetStepTypes();

    private static Exception MakeException(Type stepType) =>
      new PoezdConfigurationException(
        $"Can not find a step of type '{stepType.FullName}'. You should register this step type in your DI-container.");

    private readonly IServiceProvider _serviceProvider;
  }
}
