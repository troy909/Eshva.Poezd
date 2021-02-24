#region Usings

using System;

#endregion

namespace Venture.CaseOffice.Domain
{
  public class ResearchCase
  {
    public ResearchCase(
      Guid id,
      string reason,
      string knowledgeArea)
    {
      Id = id;
      Reason = reason;
      KnowledgeArea = knowledgeArea;
    }

    public Guid Id { get; }

    public string Reason { get; }

    public string KnowledgeArea { get; }

    public static readonly ResearchCase None = new ResearchCase(
      Guid.Empty,
      string.Empty,
      string.Empty);
  }
}
