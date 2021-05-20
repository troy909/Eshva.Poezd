namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Contract of a message handling context that can skip further message handling a pipeline.
  /// </summary>
  /// <remarks>
  /// Useful for filtering incoming messages. Will act on all message handlers.
  /// </remarks>
  public interface ICanSkipFurtherMessageHandling
  {
    /// <summary>
    /// Gets sign that this message should be skipped.
    /// </summary>
    bool ShouldSkipFurtherMessageHandling { get; }

    /// <summary>
    /// Sets sign that this message should be skipped.
    /// </summary>
    void SkipFurtherMessageHandling();
  }
}
