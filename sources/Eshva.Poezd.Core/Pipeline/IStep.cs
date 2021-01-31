#region Usings

using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Common.Collections;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  public interface IStep
  {
    Task Execute(IPocket context);
  }
}
