#region Usings

using System;

#endregion


namespace Venture.CaseOffice.Domain
{
  public class JusticeCase
  {
    public JusticeCase(Guid caseId, Guid subjectId, string reason)
    {
      _caseId = caseId;
      _subjectId = subjectId;
      _reason = reason;
    }

    public static readonly JusticeCase None = new JusticeCase(Guid.Empty, Guid.Empty, string.Empty);
    private readonly Guid _caseId;
    private readonly Guid _subjectId;
    private readonly string _reason;
  }
}
