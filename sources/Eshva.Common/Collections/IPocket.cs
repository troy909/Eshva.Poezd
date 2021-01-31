#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Eshva.Common.Collections
{
  /// <summary>
  /// The contract of a pocket full of different things.
  /// </summary>
  /// <remarks>
  /// If you put disposable thing into the pocket it's your obligation to dispose them.
  /// </remarks>
  public interface IPocket
  {
    /// <summary>
    /// Tries to take a thing from the pocket with the <paramref name="name" /> and of expected <typeparamref name="TThing" />
    /// type.
    /// </summary>
    /// <typeparam name="TThing">
    /// The expected type of the <paramref name="thing" />.
    /// </typeparam>
    /// <param name="name">
    /// The name of the required thing from the pocket.
    /// </param>
    /// <param name="thing">
    /// If a thing with a <paramref name="name" /> found contains the found thing.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="name" /> is <c>null</c>, an empty or whitespace string.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// A thing with a <paramref name="name" /> found but it isn't of type <typeparamref name="TThing" />.
    /// </exception>
    bool TryTake<TThing>([NotNull] string name, out TThing thing);

    /// <summary>
    /// Puts a <paramref name="thing" /> named <paramref name="name" /> into the pocket.
    /// </summary>
    /// <param name="name">
    /// The name of the thing to put.
    /// </param>
    /// <param name="thing">
    /// The thing to put into the pocket.
    /// </param>
    /// <returns>
    /// This pocket instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="name" /> is <c>null</c>, an empty or whitespace string, or <paramref name="thing" /> is
    /// <c>null</c>.
    /// </exception>
    [NotNull]
    IPocket Put([NotNull] string name, [NotNull] object thing);

    /// <summary>
    /// Tries to remove the thing with name <paramref name="name" />.
    /// </summary>
    /// <param name="name">
    /// The name of thing to remove.
    /// </param>
    /// <returns>
    /// <c>true</c> - a thing with the <paramref name="name" /> found and removed. <c>false</c> - a thing with
    /// <paramref name="name" /> isn't found and removed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="name" /> is <c>null</c>, an empty or whitespace string.
    /// </exception>
    bool TryRemove([NotNull] string name);

    /// <summary>
    /// Returns all the things from the pocket.
    /// </summary>
    /// <returns>
    /// A read-only dictionary which key is the name of a thing and value is the thing with this name.
    /// </returns>
    IReadOnlyDictionary<string, object> GetThings();
  }
}
