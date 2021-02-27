using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

namespace Venture.Common.Poezd.Adapter.MessagePublishing
{
  public class GetMessageKeyStep: IStep<MessagePublishingContext>
  {
    public Task Execute(MessagePublishingContext context) => throw new System.NotImplementedException();
  }
}
