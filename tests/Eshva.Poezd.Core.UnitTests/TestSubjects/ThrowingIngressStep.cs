#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class ThrowingIngressStep : IStep<IPocket>
  {
    public Task Execute(IPocket context) => throw new Exception("Sample exception from a message handler.");
  }
}
