#region Usings

using System;
using System.Collections.Concurrent;
using Venture.Common.Tools;

#endregion


namespace Venture.Common.Application.MessageHandling
{
  public class PocketExecutionContext : IPocket
  {
    public bool TryGet<TValue>(out TValue value) => TryGet(typeof(TValue).FullName, out value);

    /// <exception cref="System.InvalidCastException">
    /// Найденное значение имеет тип, отличный от ожидаемого.
    /// </exception>
    public bool TryGet<TValue>(string key, out TValue value)
    {
      if (_things.TryGetValue(key, out var found))
      {
        value = (TValue)found;
        return true;
      }

      value = default;
      return false;
    }

    /// <exception cref="ArgumentNullException">
    /// Значение не задано.
    /// </exception>
    public void Set<TValue>(TValue value) => Set(typeof(TValue).FullName, value);

    /// <exception cref="ArgumentNullException">
    /// Не задан ключ или значение, либо ключ представляет собой пустую строку или только пробельные символы.
    /// </exception>
    public void Set<TValue>(string key, TValue value)
    {
      if (string.IsNullOrWhiteSpace(key))
      {
        throw new ArgumentNullException(nameof(key));
      }

      if (Equals(value, default(TValue)))
      {
        throw new ArgumentNullException(nameof(value));
      }

      _things[key] = value;
    }

    public bool TryRemove<TValue>() => TryRemove(typeof(TValue).FullName);

    /// <exception cref="ArgumentNullException">
    /// Ключ представляет собой пустую строку или только пробельные символы.
    /// </exception>
    public bool TryRemove(string key)
    {
      if (string.IsNullOrWhiteSpace(key))
      {
        throw new ArgumentNullException(nameof(key));
      }

      return _things.TryRemove(key, out _);
    }

    private readonly ConcurrentDictionary<string, object> _things = new ConcurrentDictionary<string, object>();
  }
}
