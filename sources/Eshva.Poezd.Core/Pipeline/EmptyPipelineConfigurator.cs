namespace Eshva.Poezd.Core.Pipeline
{
  public class EmptyPipelineConfigurator : IPipelineConfigurator
  {
    public IPipeline ConfigurePipeline(IPipeline pipeline) => new MessageHandlingPipeline();
  }
}
