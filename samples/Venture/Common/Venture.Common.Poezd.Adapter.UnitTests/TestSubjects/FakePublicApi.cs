#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests.TestSubjects
{
  [SuppressMessage("ReSharper", "NotNullMemberIsNotInitialized")]
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  public class FakePublicApi : IPublicApi
  {
    public string Id { get; set; }

    public PublicApiConfiguration Configuration { get; set; }

    public IPipeFitter IngressPipeFitter { get; set; }

    public IPipeFitter EgressPipeFitter { get; set; }

    public IMessageTypesRegistry MessageTypesRegistry { get; set; }

    public IHandlerRegistry HandlerRegistry { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global
    public IEnumerable<string> QueueNamePatterns { get; set; }

    public IEnumerable<string> GetQueueNamePatterns() => QueueNamePatterns;
  }
}
