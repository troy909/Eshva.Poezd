#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  public interface IPipeline<in TStep>
  {
    IPipeline<TStep> Append(TStep step);

    void InsertBefore(TStep step);

    void InsertAfter(TStep step);

    void Remove(Type stepType);

    Task Execute(IPocket context);
  }
}
