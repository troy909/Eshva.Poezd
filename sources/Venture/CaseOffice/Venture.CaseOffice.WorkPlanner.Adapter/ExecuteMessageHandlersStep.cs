#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  /// <summary>
  /// Executes all found message handlers, collects theirs commit decisions and stores them in the message handling context.
  /// </summary>
  public class ExecuteMessageHandlersStep : IStep
  {
    public Task Execute(IPocket context) => throw new NotImplementedException();
  }
}
