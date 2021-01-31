#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  /// <summary>
  /// Deserializes message POCO from broker message serialized as FlatBuffers-table using type extracted earlier.
  /// </summary>
  public class DeserializeMessageStep : IStep
  {
    public Task Execute(IPocket context) => throw new NotImplementedException();
  }
}
