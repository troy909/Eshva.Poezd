#region Usings

using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.IntegrationTests.Tools
{
  internal interface IMessageHandler<in TMessage>
  {
    Task Handle([NotNull] TMessage message, [NotNull] TestIncomingMessageHandlingContext context);
  }
}
