#region Usings

using System.Threading.Tasks;
using Eshva.Common;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  public interface IStep
  {
    Task Execute(IPocket context);
  }
}
