#region Usings

using System.Collections.Generic;
using System.Threading.Tasks;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter
{
  public interface IHandlersExecutionStrategy
  {
    Task ExecuteHandlers(
      IEnumerable<HandlerDescriptor> handlers,
      object message,
      VentureContext context);
  }
}
