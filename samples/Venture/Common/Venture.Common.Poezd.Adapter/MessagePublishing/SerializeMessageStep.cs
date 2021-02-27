#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.MessagePublishing
{
  public class SerializeMessageStep : IStep<MessagePublishingContext>
  {
    public Task Execute(MessagePublishingContext context) => throw new NotImplementedException();
  }
}
