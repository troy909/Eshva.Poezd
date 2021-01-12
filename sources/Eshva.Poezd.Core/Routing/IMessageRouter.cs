#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Activation;
using Eshva.Poezd.Core.Configuration;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public interface IMessageRouter
  {
    Task RouteMessage(object message);
  }

  public class DefaultMessageRouter : IMessageRouter
  {
    public DefaultMessageRouter([NotNull] PoezdConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task RouteMessage(object message)
    {
      var transactionContext = new TransactionContext();
      var handlers = await _configuration.MessageHandling.MessageHandlersFactory.GetHandlersOfMessage(message, transactionContext);
      foreach (var handler in handlers)
      {
        await handler.Handle(message, transactionContext);
      }
    }

    private readonly PoezdConfiguration _configuration;
  }
}
