#region Usings

using System;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// Contract of producer registry.
  /// </summary>
  /// <remarks>
  /// The implementation should dispose all registered producers.
  /// </remarks>
  [PublicAPI]
  internal interface IProducerRegistry : IDisposable
  {
    /// <summary>
    /// Adds a <paramref name="producer" /> belonging to the egress <paramref name="api" />.
    /// </summary>
    /// <param name="api">
    /// The egress API to which the adding <paramref name="producer" /> belongs to.
    /// </param>
    /// <param name="producer">
    /// The adding producer.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    /// <exception cref="PoezdConfigurationException">
    /// An egress API and its producer already registered.
    /// </exception>
    void Add([NotNull] IEgressApi api, [NotNull] IApiProducer producer);

    /// <summary>
    /// Gets the producer that belongs to the egress <paramref name="api" />.
    /// </summary>
    /// <param name="api">
    /// The egress API the looking producer belongs to.
    /// </param>
    /// <returns>
    /// The egress API producer.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The egress API is not specified.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// There is no registered producer for the egress API specified.
    /// </exception>
    [NotNull]
    IApiProducer Get([NotNull] IEgressApi api);
  }
}
