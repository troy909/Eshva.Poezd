#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace Venture.Common.Application.Storage
{
  /// <summary>
  /// It is an in-memory stab for a real aggregate storage base class. In a real application this class should be placed in
  /// an adapter assembly that should adapt this interface to underlying storage mechanism like EventStoreDB, PostgreSQL or
  /// MongoDB.
  /// </summary>
  /// <typeparam name="TAggregate">
  /// An aggregate type to store and retrieve.
  /// </typeparam>
  public sealed class AlmostRealAggregateStorage<TAggregate> :
    IAggregateStorage<TAggregate>,
    IAggregateStorageTestBackdoor<TAggregate>
    where TAggregate : class
  {
    /// <inheritdoc />
    public Task<TAggregate> Read(Guid aggregateId)
    {
      var aggregateType = typeof(TAggregate);
      if (!AggregateTypeStreams.TryGetValue(aggregateType, out var aggregateStreams) ||
          !aggregateStreams.TryGetValue(aggregateId, out var aggregate))
        throw new KeyNotFoundException($"An aggregate of type {aggregateType.FullName} with ID {aggregateId} not found.");

      return Task.FromResult((TAggregate) aggregate);
    }

    /// <inheritdoc />
    public Task Write(Guid aggregateId, TAggregate aggregate)
    {
      var aggregateType = typeof(TAggregate);
      var aggregateStream = AggregateTypeStreams.GetOrAdd(aggregateType, new ConcurrentDictionary<Guid, object>());
      aggregateStream.AddOrUpdate(
        aggregateId,
        aggregate,
        (_, __) => aggregate);
      return Task.CompletedTask;
    }

    public IEnumerable<TAggregate> GetAll()
    {
      var aggregateType = typeof(TAggregate);
      if (!AggregateTypeStreams.TryGetValue(aggregateType, out var aggregateStreams))
        throw new KeyNotFoundException($"Aggregates of type {aggregateType.FullName} not stored in this storage.");
      return aggregateStreams.Select(pair => (TAggregate) pair.Value);
    }

    // ReSharper disable once StaticMemberInGenericType - it's just an in-memory stab.
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Guid, object>> AggregateTypeStreams =
      new ConcurrentDictionary<Type, ConcurrentDictionary<Guid, object>>();
  }

  public interface IAggregateStorageTestBackdoor<out TAggregate> where TAggregate : class
  {
    IEnumerable<TAggregate> GetAll();
  }
}
