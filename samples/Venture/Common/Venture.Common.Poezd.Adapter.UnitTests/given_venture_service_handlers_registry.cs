#region Usings

using FluentAssertions;
using Venture.Common.Poezd.Adapter.MessageHandling;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_venture_service_handlers_registry
  {
    [Fact]
    public void when_created_it_should_provide_handler_types_grouped_by_handled_message_type()
    {
      var sut = new VentureServiceHandlersRegistry(new[] {typeof(Message01Handler).Assembly});
      sut.HandlersGroupedByMessageType.ContainsKey(typeof(Message01)).Should()
        .BeTrue($"there is at least one handler for {nameof(Message01)} event");
    }
  }
}
