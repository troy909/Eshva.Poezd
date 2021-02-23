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
      string reason,
      Guid responsibleId)
    {
      CaseId = caseId;
      SubjectId = subjectId;
      Reason = reason;
      ResponsibleId = responsibleId;
    }

    public Guid CaseId { get; }

    public string Reason { get; }

    public Guid ResponsibleId { get; }

    public Guid SubjectId { get; }

    public static readonly JusticeCase None = new JusticeCase(
      Guid.Empty,
      Guid.Empty,
      string.Empty,
      Guid.Empty);
  }
}
