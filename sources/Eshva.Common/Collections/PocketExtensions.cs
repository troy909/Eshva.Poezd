#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Eshva.Common.Collections
{
  /// <summary>
  /// Extension methods for <see cref="IPocket" /> implementations.
  /// </summary>
  public static class PocketExtensions
  {
    /// <summary>
    /// Takes the thing with the given name and type from the pocket, returning null if the there is no a thing with such name.
    /// If the found thing can not be casted to the expected <typeparamref name="TThing" />, a nice exception is thrown.
    /// </summary>
    /// <typeparam name="TThing">
    /// The expected type of the thing".
    /// </typeparam>
    /// <param name="pocket">
    /// The pocket to find a thing within.
    /// </param>
    /// <param name="name">
    /// The name of thing to find.
    /// </param>
    /// <returns>
    /// Found thing.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="name" /> is <c>null</c>, an empty or whitespace string.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// A thing with <paramref name="name" /> found but it isn't of expected type <typeparamref name="TThing" />.
    /// </exception>
    [NotNull]
    public static TThing TakeOrNull<TThing>(this IPocket pocket, [NotNull] string name) where TThing : class
    {
      pocket.TryTake<TThing>(name, out var item);
      return item;
    }

    /// <summary>
    /// Takes the thing with the given <paramref name="name" /> and <typeparamref name="TThing" /> type from the pocket,
    /// throwing a nice exception if either the key does not exist, or the found item cannot be casted to the expected
    /// <typeparamref name="TThing" />.
    /// </summary>
    /// <typeparam name="TThing">
    /// The expected type of the thing".
    /// </typeparam>
    /// <param name="pocket">
    /// The pocket to find a thing within.
    /// </param>
    /// <param name="name">
    /// The name of thing to find.
    /// </param>
    /// <returns>
    /// Found thing.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="name" /> is <c>null</c>, an empty or whitespace string.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Could not find a thing with the given <paramref name="name" />.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// A thing with <paramref name="name" /> found but it isn't of expected type <typeparamref name="TThing" />.
    /// </exception>
    [NotNull]
    public static TThing TakeOrThrow<TThing>(this IPocket pocket, [NotNull] string name) where TThing : class
    {
      if (!pocket.TryTake<TThing>(name, out var item)) throw new KeyNotFoundException($"Could not find an item with the key '{name}'.");
      return item;
    }

    /// <summary>
    /// Takes a thing from the pocket if it stored in it or creates a thing using <paramref name="thingFactory" />, puts it
    /// into the pocket and returns it.
    /// </summary>
    /// <typeparam name="TThing">
    /// The expected type of the thing".
    /// </typeparam>
    /// <param name="pocket">
    /// The pocket to find a thing within.
    /// </param>
    /// <param name="name">
    /// The name of thing to find.
    /// </param>
    /// <param name="thingFactory">
    /// The factory that will be used to make a thing if it is not found in the pocket.
    /// </param>
    /// <returns>
    /// The found or a made thing.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="thingFactory" /> is <c>null</c> or <paramref name="name" /> is <c>null</c> an empty or whitespace
    /// string.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Could not find a thing with the given <paramref name="name" />.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// A thing with <paramref name="name" /> found but it isn't of expected type <typeparamref name="TThing" />.
    /// </exception>
    [NotNull]
    public static TThing TakeOrPut<TThing>(
      this IPocket pocket,
      [NotNull] string name,
      [NotNull] Func<TThing> thingFactory) where TThing : class
    {
      if (thingFactory == null) throw new ArgumentNullException(nameof(thingFactory));
      if (pocket.TryTake<TThing>(name, out var thing)) return thing;

      var madeThing = thingFactory();
      pocket.Put(name, madeThing);
      return madeThing;
    }
  }
}
