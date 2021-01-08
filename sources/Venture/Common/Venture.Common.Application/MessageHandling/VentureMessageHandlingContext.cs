namespace Venture.Common.Application.MessageHandling
{
  /// <summary>
  /// Представляет собой реализацию контекста обработки сообщения внутри продукта Venture.
  /// </summary>
  public class VentureMessageHandlingContext : PocketExecutionContext, IMessageHandlingContext
  {
    public void Commit()
    {
      // TODO: Реализовать подтверждение обработки сообщения.
    }

    public void Abort()
    {
      // TODO: Реализовать прерывание обработки сообщения.
    }

    public static class Keys
    {
      public const string MessageId = "MessageId";
      public const string MessageType = "MessageType";
      public const string SourceTopic = "SourceTopic";
      public const string CorrelationId = "CorrelationId";
      public const string CausationId = "CausationId";
      public const string SentOnUtc = "SentOnUtc";
      public const string DateTimeOffset = "DateTimeOffset";
    }
  }
}
