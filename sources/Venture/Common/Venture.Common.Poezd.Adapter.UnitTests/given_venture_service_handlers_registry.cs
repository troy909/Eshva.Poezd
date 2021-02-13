#region Usings

using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;
using Venture.WorkPlanner.Messages.V1.Events;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class given_work_planner_handlers_registry
  {
    [Fact]
    public void when_created_it_should_provide_handler_types_grouped_by_handled_message_type()
    {
      var sut = new VentureServiceHandlersRegistry(new[] {typeof(Message01Handler).Assembly}, typeof(Message01).Namespace);
      sut.HandlersGroupedByMessageType.ContainsKey(typeof(Message01)).Should()
        .BeTrue($"there is at least one handler for {nameof(Message01)} event");
    }
  }
}
