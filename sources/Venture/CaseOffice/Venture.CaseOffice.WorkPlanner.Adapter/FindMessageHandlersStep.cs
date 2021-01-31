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
  /// Finds all message handlers for a message stored as a POCO in the message handling context.
  /// </summary>
  public class FindMessageHandlersStep : IStep
  {
    public Task Execute(IPocket context) => throw new NotImplementedException();
  }
}
