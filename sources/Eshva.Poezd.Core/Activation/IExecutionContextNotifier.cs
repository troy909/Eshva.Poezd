namespace Eshva.Poezd.Core.Activation
{
  public interface IExecutionContextNotifier
  {
    IExecutionEventSubscriptions SubscribeOn { get; }
  }
}
