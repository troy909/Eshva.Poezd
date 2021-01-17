#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class MessageBrokerConfiguration
  {
    public string Id { get; internal set; }

    public string Name { get; internal set; }

    public IReadOnlyList<ExternalServiceConfiguration> ExternalServices => _externalServices;

    public Type QueueNameMatcherType { get; internal set; }

    public Type AdapterType { get; set; }

    public void AddExternalService([NotNull] ExternalServiceConfiguration externalServiceConfiguration)
    {
      if (externalServiceConfiguration == null) throw new ArgumentNullException(nameof(externalServiceConfiguration));

      _externalServices.Add(externalServiceConfiguration);
    }

    private readonly List<ExternalServiceConfiguration> _externalServices = new List<ExternalServiceConfiguration>();
  }
}
