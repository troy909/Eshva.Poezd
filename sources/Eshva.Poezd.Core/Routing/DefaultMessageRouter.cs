using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Activation;
using Eshva.Poezd.Core.Configuration;
using JetBrains.Annotations;


namespace Eshva.Poezd.Core.Routing
{
  public sealed class DefaultMessageRouter : IMessageRouter
  {
    public DefaultMessageRouter([NotNull] PoezdConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task RouteIncomingMessage(object message, ITransactionContext transactionContext)
    {
      if (message == null) throw new ArgumentNullException(nameof(message));
      if (transactionContext == null) throw new ArgumentNullException(nameof(transactionContext));

      var handlers = await _configuration.MessageHandling.MessageHandlersFactory.GetHandlersOfMessage(message, transactionContext);
      foreach (var handler in handlers)
      {
        await handler.Handle(message, transactionContext);
      }
    }

    private readonly PoezdConfiguration _configuration;
  }
}