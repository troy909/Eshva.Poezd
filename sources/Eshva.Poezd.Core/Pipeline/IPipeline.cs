#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Common.Collections;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  public interface IPipeline
  {
    IPipeline Append(IStep step);

    IPipeline Append(IEnumerable<IStep> steps);

    void InsertBefore(IStep step);

    void InsertAfter(IStep step);

    void Remove(Type stepType);

    Task Execute(IPocket context);
  }
}
