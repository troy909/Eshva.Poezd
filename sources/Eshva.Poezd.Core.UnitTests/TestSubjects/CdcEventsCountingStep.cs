using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.Pipeline;


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public sealed class CdcEventsCountingStep : IStep
  {
    public Task Execute(IPocket context)
    {
      return null;
    }
  }
}