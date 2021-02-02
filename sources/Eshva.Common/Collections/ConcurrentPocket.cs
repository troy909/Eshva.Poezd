#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

#endregion

namespace Eshva.Common.Collections
{
  /// <summary>
  /// A pocket with concurrent access.
  /// </summary>
  /// <remarks>
  /// If you put disposable thing into the pocket it's your obligation to dispose them.
  /// </remarks>
  public class ConcurrentPocket : IPocket
  {
    /// <inheritdoc />
    public bool TryTake<TValue>(string name, out TValue thing)
    {
      if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

      if (_things.TryGetValue(name, out var found))
      {
        // ReSharper disable once JoinNullCheckWithUsage The R# is wrong here with suggestion because TValue has no a 'class' constraint.
        if (!(found is TValue))
        {
          throw new InvalidCastException(
            $"Found item with name '{name}' but it was a {found.GetType()} and not of type {typeof(TValue)} as expected.");
        }

        thing = (TValue) found;
        return true;
      }

      thing = default;
      return false;
    }

    /// <inheritdoc />
    public IPocket Put(string name, object thing)
    {
      if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

      _things[name] = thing ?? throw new ArgumentNullException(nameof(thing));

      return this;
    }

    /// <inheritdoc />
    public bool TryRemove(string name)
    {
      if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

      return _things.TryRemove(name, out _);
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object> GetThings() => _things;

    private readonly ConcurrentDictionary<string, object> _things = new ConcurrentDictionary<string, object>();
  }
}
