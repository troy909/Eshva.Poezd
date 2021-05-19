#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class TestDriverState
  {
    public int InitializedCount { get; set; }

    public int MessageConsumingStartedCount { get; set; }

    public List<string> SubscribedQueueNamePatters { get; } = new List<string>();

    public int DisposedCount { get; set; }

    public int PublishedMessageCount { get; set; }

    public MessagePublishingContext PublishingContext { get; set; }
  }
}
