#region Usings

using System.Collections.Generic;
using System.Threading.Tasks;
using Venture.Common.Application.Ingress;

#endregion

namespace Venture.Common.Poezd.Adapter.Ingress
{
  public interface IHandlersExecutionStrategy
  {
    Task ExecuteHandlers(
      IEnumerable<HandlerDescriptor> handlers,
      object message,
      VentureIncomingMessageHandlingContext context);
  }
}
