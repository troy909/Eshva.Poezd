#region Usings

using FluentAssertions;
using Venture.WorkPlanner.Messages.V1.Events;
using Xunit;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter.UnitTests
{
  public class given_work_planner_handlers_registry
  {
    [Fact]
    public void when_created_it_should_provide_handler_types_grouped_by_handled_message_type()
    {
      var sut = new WorkPlannerHandlersRegistry();
      sut.HandlersGroupedByMessageType.ContainsKey(typeof(TaskCreated)).Should()
        .BeTrue($"there is at least one handler for {nameof(TaskCreated)} event");
    }
  }
}
