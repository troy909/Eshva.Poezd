#region Usings

using System;

#endregion


namespace Venture.CaseOffice.Domain
{
  public class JusticeCase
  {
    public JusticeCase(
      Guid caseId,
      Guid subjectId,
      string reason)
    {
      _caseId = caseId;
      _subjectId = subjectId;
      _reason = reason;
    }

    private readonly Guid _caseId;
    private readonly string _reason;
    private readonly Guid _subjectId;

    public static readonly JusticeCase None = new JusticeCase(
      Guid.Empty,
      Guid.Empty,
      string.Empty);
  }
}
