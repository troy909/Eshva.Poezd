namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  public static class WorkPlannerApi
  {
    /// <summary>
    /// The header keys that uses the Work Planner service for a message metadata in Kafka message headers.
    /// </summary>
    public static class Headers
    {
      public const string MessageId = "id";
      public const string CorrelationId = "correlation-id";
      public const string CausationId = "causation-id";
    }
  }
}
