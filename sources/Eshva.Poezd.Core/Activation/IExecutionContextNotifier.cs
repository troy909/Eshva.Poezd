namespace Eshva.Poezd.Core.Activation
{
  public interface IExecutionContextNotifier
  {
    IExecutionEventsSubscriptions SubscribeOn { get; }
  }
}
