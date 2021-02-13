#region Usings

using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.Application.MessageHandling
{
  public interface ICanCommit<in TMessage>
  {
    Task Commit([NotNull] TMessage message, [NotNull] VentureContext context);
  }
}
