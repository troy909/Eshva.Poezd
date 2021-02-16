namespace Venture.CaseOffice.Messages
{
  /// <summary>
  /// Case Office messages assembly tag.
  /// </summary>
  /// <remarks>
  /// Used to identify this assembly when searching for types in this assembly with reflection.
  /// </remarks>
  public static class Api
  {
    public const string V1Namespace = "Venture.CaseOffice.Messages.V1";

    /// <summary>
    /// The header keys that uses the Work Planner service for a message metadata in Kafka message headers.
    /// </summary>
    public static class Headers
    {
      public const string MessageTypeName = "type";
      public const string MessageId = "id";
      public const string CorrelationId = "correlation-id";
      public const string CausationId = "causation-id";
    }
  }
}
