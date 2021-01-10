#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public interface IManageTransaction
  {
    Task Commit();

    Task Rollback();
  }
}
