#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  /// <summary>
  /// Extracts authorization metadata from API message metadata stored as <see cref="OriginatorToken" />.
  /// </summary>
  public class ExtractAuthorizationMetadataStep : IStep
  {
    public Task Execute(IPocket context) => throw new NotImplementedException();
  }
}
