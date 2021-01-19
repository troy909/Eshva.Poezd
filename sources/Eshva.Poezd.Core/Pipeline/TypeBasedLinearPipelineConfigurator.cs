#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  public abstract class TypeBasedLinearPipelineConfigurator : IPipelineConfigurator
  {
    protected TypeBasedLinearPipelineConfigurator([NotNull] IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IPipeline ConfigurePipeline(IPipeline pipeline)
    {
      foreach (var stepType in GetStepTypes())
      {
        pipeline.Append((IStep)_serviceProvider.GetService(stepType, MakeException));
      }

      return pipeline;
    }

    protected abstract IEnumerable<Type> GetStepTypes();

    private static Exception MakeException(Type stepType) =>
      new PoezdConfigurationException(
        $"Can not find a step of type '{stepType.FullName}'. You should register this step type in your DI-container.");

    private readonly IServiceProvider _serviceProvider;
  }
}
