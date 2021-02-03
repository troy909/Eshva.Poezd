#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Venture.WorkPlanner.Messages.V1.Events;
using Xunit;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter.UnitTests
{
  public class given_extract_message_type_step
  {
    [Fact]
    public async Task when_executed_with_context_containing_message_type_within_broker_message_metadata_it_should_store_it_in_context()
    {
      var sut = new ExtractMessageTypeStep();
      var context = new ConcurrentPocket();
      const string expectedTypeName = "Venture.WorkPlanner.Messages.V1.Events.TaskCreated";
      context.Put(
        ContextKeys.Broker.MessageMetadata,
        new Dictionary<string, string> {{WorkPlannerApi.Headers.MessageTypeName, expectedTypeName}});

      await sut.Execute(context);

      context.TakeOrNull<string>(ContextKeys.Application.MessageTypeName)
        .Should().Be(expectedTypeName, "this header should be copied");
      context.TakeOrNull<Type>(ContextKeys.Application.MessageType)
        .Should().Be(typeof(TaskCreated), "message type should be recognized by its name");
    }

    [Fact]
    public void when_executed_with_context_with_missing_message_type_within_broker_message_metadata_it_should_throw()
    {
      var step = new ExtractMessageTypeStep();
      var context = new ConcurrentPocket();
      context.Put(ContextKeys.Broker.MessageMetadata, new Dictionary<string, string>());

      Func<Task> sut = () => step.Execute(context);
      sut.Should().Throw<PoezdSkipMessageException>("messages of unknown type should be skipped");
    }

    [Fact]
    public void when_executed_with_context_with_wrong_message_type_within_broker_message_metadata_it_should_throw()
    {
      var step = new ExtractMessageTypeStep();
      var context = new ConcurrentPocket();

      context.Put(ContextKeys.Broker.MessageMetadata, new Dictionary<string, string> {{WorkPlannerApi.Headers.MessageTypeName, null}});
      Func<Task> sut = () => step.Execute(context);

      sut.Should().Throw<PoezdSkipMessageException>("messages of unknown type should be skipped");
      context.Put(
        ContextKeys.Broker.MessageMetadata,
        new Dictionary<string, string> {{WorkPlannerApi.Headers.MessageTypeName, string.Empty}});
      sut = () => step.Execute(context);
      sut.Should().Throw<PoezdSkipMessageException>("messages of unknown type should be skipped");
      context.Put(
        ContextKeys.Broker.MessageMetadata,
        new Dictionary<string, string> {{WorkPlannerApi.Headers.MessageTypeName, WhitespaceString}});
      sut = () => step.Execute(context);
      sut.Should().Throw<PoezdSkipMessageException>("messages of unknown type should be skipped");
    }

    [Fact]
    public void when_executed_with_context_with_unknown_message_type_within_broker_message_metadata_it_should_throw_skip_message_exception()
    {
      var step = new ExtractMessageTypeStep();
      var context = new ConcurrentPocket();

      context.Put(ContextKeys.Broker.MessageMetadata, new Dictionary<string, string> {{WorkPlannerApi.Headers.MessageTypeName, "unknown"}});
      Func<Task> sut = () => step.Execute(context);

      sut.Should().Throw<PoezdSkipMessageException>();
    }

    [Fact]
    public void when_executed_without_context_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute it's a test
      Func<Task> sut = async () => await new ExtractMessageTypeStep().Execute(context: null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("context"), "context must be specified");
    }

    private const string WhitespaceString = " \t\n\r";
  }
}
