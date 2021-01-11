#region Usings

using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion


namespace Venture.Common.Application.MessageHandling
{
  [UsedImplicitly(ImplicitUseTargetFlags.Itself | ImplicitUseTargetFlags.WithInheritors)]
  public interface IHandleMessageOfType<in TMessage>
  {
    Task Handle(TMessage message, VentureMessageHandlingContext context);
  }
}
