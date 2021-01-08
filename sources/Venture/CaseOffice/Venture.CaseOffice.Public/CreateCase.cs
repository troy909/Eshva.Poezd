#region Usings

using System;

#endregion


namespace Venture.CaseOffice.Public
{
  public class CreateCase
  {
    public string CaseType { get; set; }

    public Guid SubjectId { get; set; }

    public Guid CaseId { get; set; }

    public string Reason { get; set; }
  }
}
