#region Usings

using System.Collections.Generic;
using System.Threading.Tasks;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  public interface IHandlersExecutionPolicy
  {
    Task ExecuteHandlers(
      IEnumerable<HandlerDescriptor> handlers,
      object message,
      VentureContext messageHandlingContext);
  }
}
