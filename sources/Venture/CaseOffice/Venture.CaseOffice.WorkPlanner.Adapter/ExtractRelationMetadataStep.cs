#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  /// <summary>
  /// Extracts correlation ID, causation ID from broker message headers and sets appropriate metadata in the message handling
  /// context.
  /// </summary>
  public class ExtractRelationMetadataStep : IStep
  {
    public Task Execute(IPocket context) => throw new NotImplementedException();
  }
}
