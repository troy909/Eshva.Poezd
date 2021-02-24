namespace Venture.Common.Poezd.Adapter
{
  /// <summary>
  /// Work Planner JWT token structure.
  /// </summary>
  /// <remarks>
  /// TODO: TBD Should I make it a struct?
  /// </remarks>
  public sealed class OriginatorToken
  {
    /// <summary>
    /// Authentication source user ID.
    /// </summary>
    public string UserId;

    /// <summary>
    /// Authentication source user roles.
    /// </summary>
    public string[] UserRoleIds;
  }
}
