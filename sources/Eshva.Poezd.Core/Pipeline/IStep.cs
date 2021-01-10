using System.Threading.Tasks;
using Eshva.Common;


namespace Eshva.Poezd.Core.Pipeline
{
  public interface IStep
  {
    Task ExecuteWithinContext(IPocket context, IStep nextStep);
  }
}