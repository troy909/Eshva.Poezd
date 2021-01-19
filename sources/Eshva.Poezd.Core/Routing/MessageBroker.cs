#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Configuration;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public sealed class MessageBroker
  {
    public MessageBroker(
      [NotNull] IMessageBrokerDriver driver,
      [NotNull] MessageBrokerConfiguration configuration)
    {
      Driver = driver ?? throw new ArgumentNullException(nameof(driver));
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      PublicApis = configuration.PublicApis.Select(apiConfiguration => new PublicApi(apiConfiguration)).ToList().AsReadOnly();
    }

    public IMessageBrokerDriver Driver { get; }

    public string Id => _configuration.Id;

    public IReadOnlyCollection<PublicApi> PublicApis { get; }

    private readonly MessageBrokerConfiguration _configuration;
  }
}
