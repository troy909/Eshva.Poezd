#region Usings

using System.Collections.Generic;
using Eshva.Common.Collections;

#endregion

namespace Eshva.Poezd.Core.Common
{
  public static class PocketExtensions
  {
    public static KeyNotFoundException MakeKeyNotFoundException(this IPocket _, string propertyName) =>
      new($"Could not find an item with the key '{propertyName}'.");
  }
}
