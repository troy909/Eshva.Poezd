#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Configuration of a message broker.
  /// </summary>
  public sealed class MessageBrokerConfiguration : CompositeMessageRouterConfigurationPart
  {
    /// <summary>
    /// Gets ID of the message broker.
    /// </summary>
    public string Id { get; internal set; }

    /// <summary>
    /// The list of public APIs hosted by this message broker.
    /// </summary>
    public IReadOnlyCollection<PublicApiConfiguration> PublicApis => _publicApis.AsReadOnly();

    /// <summary>
    /// Gets the queue name matcher type.
    /// </summary>
    public Type QueueNameMatcherType { get; internal set; }

    /// <summary>
    /// Gets the ingress pipe fitter type.
    /// </summary>
    public Type IngressEnterPipeFitterType { get; internal set; }

    /// <summary>
    /// Gets the ingress pipe fitter type.
    /// </summary>
    public Type IngressExitPipeFitterType { get; internal set; }

    /// <summary>
    /// Gets the driver factory type.
    /// </summary>
    public Type DriverFactoryType { get; internal set; }

    /// <summary>
    /// Gets the driver configuration.
    /// </summary>
    public object DriverConfiguration { get; internal set; }

    /// <summary>
    /// Adds a public API configuration
    /// </summary>
    /// <param name="configuration">
    /// Public API configuration to add.
    /// </param>
    public void AddPublicApi([NotNull] PublicApiConfiguration configuration)
    {
      if (configuration == null) throw new ArgumentNullException(nameof(configuration));

      _publicApis.Add(configuration);
    }

    /// <inheritdoc />
    protected override IEnumerable<string> ValidateItself()
    {
      if (string.IsNullOrWhiteSpace(Id))
        yield return "ID of the message broker should be specified.";
      if (!_publicApis.Any())
        yield return $"At least one public API should be configured for the message broker with ID '{Id}'.";
      if (QueueNameMatcherType == null)
        yield return $"The queue name matcher type should be set for the message broker with ID '{Id}'.";
      if (IngressEnterPipeFitterType == null)
        yield return $"The ingress enter pipe fitter type should be set for the message broker with ID '{Id}'.";
      if (IngressExitPipeFitterType == null)
        yield return $"The ingress exit pipe fitter type should be set for the message broker with ID '{Id}'.";
      if (DriverFactoryType == null)
        yield return $"The message broker driver factory type should be set for the message broker with ID '{Id}'.";
      if (DriverConfiguration == null)
        yield return $"The message broker driver configuration should be set for the message broker with ID '{Id}'.";
    }

    /// <inheritdoc />
    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations() => _publicApis.AsReadOnly();

    private readonly List<PublicApiConfiguration> _publicApis = new();
  }
}
