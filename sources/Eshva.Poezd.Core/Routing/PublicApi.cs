#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Configuration;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public sealed class PublicApi
  {
    public PublicApi([NotNull] PublicApiConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public string Id => _configuration.Id;

    public IReadOnlyCollection<string> QueueNamePatterns => _configuration.QueueNamePatterns;

    private readonly PublicApiConfiguration _configuration;
  }
}
