#region Usings

using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB
{
  public interface IHeaderValueCodec
  {
    [Pure] [NotNull] string Decode([CanBeNull] byte[] value);

    [Pure] [NotNull] byte[] Encode([CanBeNull] string value);
  }
}
