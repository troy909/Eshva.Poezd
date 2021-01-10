#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Eshva.Common;

#endregion


namespace Eshva.Poezd.Core.Transport
{
  public static class PocketExtensions
  {
    /// <summary>
    /// Gets the item with the given key and type from the dictionary of objects, returning null if the key does not exist.
    /// If the key exists, but the object could not be cast to the given type, a nice exception is throws.
    /// </summary>
    public static TItem GetOrNull<TItem>(this IPocket context, string key) where TItem : class
    {
      if (!context.TryGet<TItem>(key, out var item))
      {
        return default(TItem);
      }

      if (item == null)
      {
        throw new ArgumentException(
          $"Found item with key '{key}' but it was a {item.GetType()} and not of type {typeof(TItem)} as expected.");
      }

      return item;
    }

    /// <summary>
    /// Gets the item with the given key and type from the dictionary of objects, throwing a nice exception if either the key
    /// does not exist, or the found value cannot be cast to the given type/
    /// </summary>
    public static TItem GetOrThrow<TItem>(this IPocket context, string key) where TItem : class
    {
      if (!context.TryGet<TItem>(key, out var item))
      {
        throw new KeyNotFoundException($"Could not find an item with the key '{key}'.");
      }

      if (item == null)
      {
        throw new ArgumentException(
          $"Found item with key '{key}' but it was a {item.GetType()} and not of type {typeof(TItem)} as expected.");
      }

      return item;
    }

    /// <summary>
    /// Provides a shortcut to the transaction context's
    /// <see cref="ConcurrentDictionary{TKey,TValue}.GetOrAdd(TKey,System.Func{TKey,TValue})"/>,  only as a typed version that.
    /// </summary>
    public static TItem GetOrAdd<TItem>(this IPocket context, string key, Func<TItem> newItemFactory) where TItem : class
    {
      try
      {
        context.Set(key, newItemFactory());
        context.TryGet<TItem>(key, out var item);
        return item;
      }
      catch (Exception exception)
      {
        throw new Exception($"Could not 'GetOrAdd' item with key '{key}' as type {typeof(TItem)}.", exception);
      }
    }
  }
}
