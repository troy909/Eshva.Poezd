#region Usings

using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.Pipeline;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public sealed class CdcNotificationsCommitStep : IStep
  {
    public Task Execute(IPocket context)
    {
      return null;
    }
  }
}
