#region Usings

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.Application.Storage
{
  /// <summary>
  /// Contract of an adapter to a storage for aggregates.
  /// </summary>
  /// <typeparam name="TAggregate">
  /// An aggregate type to store and retrieve.
  /// </typeparam>
  public interface IAggregateStorage<TAggregate> where TAggregate : class
  {
    /// <summary>
    /// Creates an aggregate instance with the state read from underlying storage.
    /// </summary>
    /// <remarks>
    /// In a real adapter:
    /// * this method should take an optional cancellation token as an argument.
    /// * an exception defined in the application layer should be thrown if errors in underlying occurred.
    /// </remarks>
    /// <param name="aggregateId">
    /// An aggregate Id which state to retrieve.
    /// </param>
    /// <returns>
    /// A task of aggregate instance with the current state of aggregate with ID <paramref name="aggregateId" />.
    /// </returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">
    /// An aggregate with ID <paramref name="aggregateId" /> is not found in the underlying storage.
    /// </exception>
    [NotNull]
    Task<TAggregate> Read(Guid aggregateId);

    /// <summary>
    /// Stores the <paramref name="aggregate" /> state into underlying storage.
    /// </summary>
    /// <remarks>
    /// In a real adapter:
    /// * this method should take an optional cancellation token as an argument.
    /// * an exception defined in the application layer should be thrown if errors in underlying occurred.
    /// * Each aggregate type should have an ID property of AggregateTypeId type. So there should not be aggregateId parameter.
    /// </remarks>
    /// <param name="aggregateId">
    /// ID of aggregate which state to be stored from <paramref name="aggregate" /> instance.
    /// </param>
    /// <param name="aggregate">
    /// Aggregate instance which state should be stored for aggregate with <paramref name="aggregateId" />.
    /// </param>
    /// <returns>
    /// A task that could be used to wait for storing is finished.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// The aggregate instance is not specified.
    /// </exception>
    [NotNull]
    Task Write(Guid aggregateId, [NotNull] TAggregate aggregate);
  }
}
