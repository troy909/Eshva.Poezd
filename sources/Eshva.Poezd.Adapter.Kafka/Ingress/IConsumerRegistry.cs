#region Usings

using System;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// Contract of consumer registry.
  /// </summary>
  /// <remarks>
  /// The implementation should dispose all registered consumers.
  /// </remarks>
  [PublicAPI]
  internal interface IConsumerRegistry : IDisposable
  {
    /// <summary>
    /// Adds a <paramref name="consumer" /> belonging to the ingress <paramref name="api" />.
    /// </summary>
    /// <typeparam name="TKey">
    /// The message key type.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The message payload type.
    /// </typeparam>
    /// <param name="api">
    /// The ingress API to which the adding <paramref name="consumer" /> belongs to.
    /// </param>
    /// <param name="consumer">
    /// The adding consumer.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    /// <exception cref="PoezdConfigurationException">
    /// An ingress API and its consumer already registered.
    /// </exception>
    void Add<TKey, TValue>(IIngressApi api, IApiConsumer<TKey, TValue> consumer);

    /// <summary>
    /// Gets the consumer that belongs to the ingress <paramref name="api" />.
    /// </summary>
    /// <typeparam name="TKey">
    /// The message key type.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The message payload type.
    /// </typeparam>
    /// <param name="api">
    /// The ingress API the looking consumer belongs to.
    /// </param>
    /// <returns>
    /// The ingress API consumer.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The ingress API is not specified.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// There is no registered consumer for the ingress API specified.
    /// </exception>
    [NotNull]
    IApiConsumer<TKey, TValue> Get<TKey, TValue>(IIngressApi api);
  }
}
