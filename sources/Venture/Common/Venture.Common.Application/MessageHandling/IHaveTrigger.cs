namespace Venture.Common.Application.MessageHandling
{
  /// <summary>
  /// Контракт обработчика (process manager) сообщения, которое запускает процесс.
  /// </summary>
  /// <typeparam name="TMessage">
  /// Тип сообщения, являющегося триггером процесса.
  /// </typeparam>
  public interface IHaveTrigger<in TMessage> : IHandleMessageOfType<TMessage> where TMessage : class
  {
  }
}
