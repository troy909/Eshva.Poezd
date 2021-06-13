#region Usings

using Xunit;

#endregion

namespace Venture.Common.TestingTools.IntegrationTests
{
  [CollectionDefinition(Name)]
  public class EventStoreSetupCollection : ICollectionFixture<EventStoreSetupContainerAsyncFixture>
  {
    public const string Name = nameof(EventStoreSetupCollection);
  }
}
