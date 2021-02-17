#region Usings

using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.Common.Poezd.Adapter
{
  /// <summary>
  /// Extracts authorization metadata from API message metadata stored as <see cref="OriginatorToken" />.
  /// </summary>
  public class ExtractAuthorizationMetadataStep : IStep
  {
    public Task Execute(IPocket context) =>
      // TODO: Add authorization emulation later.
      Task.CompletedTask;
  }
}
