#region Usings

using System;

#endregion

namespace Venture.CaseOffice.Messages.Types
{
  // ReSharper disable once InconsistentNaming
  public partial class uuid
  {
    public static implicit operator Guid(uuid id) => new Guid(id.value);

    public static implicit operator string(uuid id) => new Guid(id.value).ToString("N");

    public static implicit operator uuid(Guid id) => new uuid {value = id.ToByteArray()};
  }
}
