#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

#endregion


namespace Eshva.Common
{
  public class PocketExecutionContext : IPocket
  {
    public bool TryGet<TValue>(out TValue value) => TryGet(typeof(TValue).FullName, out value);

    public bool TryGet<TValue>(string key, out TValue value)
    {
      if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

      if (_things.TryGetValue(key, out var found))
      {
        value = (TValue) found;
        return true;
      }

      value = default;
      return false;
    }

    public IPocket Set<TValue>(TValue value) => Set(typeof(TValue).FullName, value);

    public IPocket Set<TValue>(string key, TValue value)
    {
      if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

      if (Equals(value, default(TValue))) throw new ArgumentNullException(nameof(value));

      if (_things.ContainsKey(key))
        throw new ArgumentException($"The value with '{key}' already set. It's forbidden to change value in a pocket.");

      _things[key] = value;

      return this;
    }

    public bool TryRemove<TValue>() => TryRemove(typeof(TValue).FullName);

    public bool TryRemove(string key)
    {
      if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

      return _things.TryRemove(key, out _);
    }

    public IEnumerable<KeyValuePair<string, object>> GetItems() => _things;

    private readonly ConcurrentDictionary<string, object> _things = new ConcurrentDictionary<string, object>();
  }
}
