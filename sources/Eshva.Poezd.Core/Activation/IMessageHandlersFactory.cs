#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.MessageHandling;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public interface IMessageHandlersFactory
  {
    IEnumerable<IHandleMessage> GetHandlersOfMessage(Type messageType, IMessageHandlingContext messageHandlingContext);
  }
}
