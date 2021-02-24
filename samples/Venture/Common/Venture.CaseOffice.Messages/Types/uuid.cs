#region Usings

using System;

#endregion

namespace Venture.CaseOffice.Messages.Types
{
  public partial class uuid
  {
    public static implicit operator Guid(uuid id) => new Guid(id.value);

    public static implicit operator uuid(Guid id) => new uuid {value = id.ToByteArray()};
  }
}
