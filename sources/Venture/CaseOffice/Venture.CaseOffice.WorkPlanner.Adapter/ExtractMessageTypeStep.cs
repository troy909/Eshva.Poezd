#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  /// <summary>
  /// Extracts message type from message broker headers and sets appropriate metadata in the message handling context.
  /// </summary>
  public class ExtractMessageTypeStep : IStep
  {
    public Task Execute(IPocket context) => throw new NotImplementedException();
  }
}
