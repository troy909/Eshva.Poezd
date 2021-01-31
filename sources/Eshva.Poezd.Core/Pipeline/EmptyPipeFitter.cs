namespace Eshva.Poezd.Core.Pipeline
{
  public class EmptyPipeFitter : IPipeFitter
  {
    public IPipeline Setup(IPipeline pipeline) => new MessageHandlingPipeline();
  }
}
