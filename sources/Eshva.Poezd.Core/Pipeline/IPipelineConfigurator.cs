namespace Eshva.Poezd.Core.Pipeline
{
  public interface IPipelineConfigurator
  {
    IPipeline ConfigurePipeline(IPipeline pipeline);
  }
}
