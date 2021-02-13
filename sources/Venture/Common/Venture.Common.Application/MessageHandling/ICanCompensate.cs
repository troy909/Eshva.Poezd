#region Usings

using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.Application.MessageHandling
{
  public interface ICanCompensate<in TMessage>
  {
    Task Compensate([NotNull] TMessage message, [NotNull] VentureContext context);
  }
}
