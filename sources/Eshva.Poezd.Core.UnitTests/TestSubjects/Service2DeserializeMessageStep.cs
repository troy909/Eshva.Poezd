#region Usings

using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.Pipeline;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public sealed class Service2DeserializeMessageStep : IStep
  {
    public Task Execute(IPocket context)
    {
      return Task.CompletedTask;
    }
  }
}
