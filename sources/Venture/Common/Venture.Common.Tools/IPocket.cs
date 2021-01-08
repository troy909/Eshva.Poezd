namespace Venture.Common.Tools
{
  public interface IPocket
  {
    bool TryGet<TValue>(out TValue value);

    bool TryGet<TValue>(string key, out TValue value);

    void Set<TValue>(TValue value);

    void Set<TValue>(string key, TValue value);

    bool TryRemove<TValue>();

    bool TryRemove(string key);
  }
}
