#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Xunit;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter.UnitTests
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class given_extract_relation_metadata_step
  {
    [Fact]
    public async Task when_executed_with_context_containing_message_id_within_broker_message_metadata_it_should_store_it_in_context()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new ConcurrentPocket();
      const string expectedId = "expected";
      context.Put(
        ContextKeys.Broker.MessageMetadata,
        new Dictionary<string, string> {{WorkPlannerApi.Headers.MessageId, expectedId}});
      await sut.Execute(context);

      context.TakeOrNull<string>(ContextKeys.Application.MessageId).Should().Be(expectedId, "this header should be copied");
    }

    [Fact]
    public async Task when_executed_with_context_containing_message_metadata_with_missing_message_id_it_should_skip()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new ConcurrentPocket();
      context.Put(ContextKeys.Broker.MessageMetadata, new Dictionary<string, string>());
      await sut.Execute(context);

      context.TakeOrNull<string>(ContextKeys.Application.MessageId).Should().BeNull("this header not present");
    }

    [Fact]
    public async Task when_executed_with_context_containing_correlation_id_within_broker_message_metadata_it_should_store_it_in_context()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new ConcurrentPocket();
      const string expectedCorrelationId = "expected";
      context.Put(
        ContextKeys.Broker.MessageMetadata,
        new Dictionary<string, string> {{WorkPlannerApi.Headers.CorrelationId, expectedCorrelationId}});
      await sut.Execute(context);

      context.TakeOrNull<string>(ContextKeys.Application.CorrelationId).Should().Be(expectedCorrelationId, "this header should be copied");
    }

    [Fact]
    public async Task when_executed_with_context_containing_message_metadata_with_missing_correlation_id_it_should_skip()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new ConcurrentPocket();
      context.Put(ContextKeys.Broker.MessageMetadata, new Dictionary<string, string>());
      await sut.Execute(context);

      context.TakeOrNull<string>(ContextKeys.Application.CorrelationId).Should().BeNull("this header not present");
    }

    [Fact]
    public async Task when_executed_with_context_containing_causation_id_within_broker_message_metadata_it_should_store_it_in_context()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new ConcurrentPocket();
      const string expectedCausationId = "expected";
      context.Put(
        ContextKeys.Broker.MessageMetadata,
        new Dictionary<string, string> {{WorkPlannerApi.Headers.CausationId, expectedCausationId}});
      await sut.Execute(context);

      context.TakeOrNull<string>(ContextKeys.Application.CausationId).Should().Be(expectedCausationId, "this header should be copied");
    }

    [Fact]
    public async Task when_executed_with_context_containing_message_metadata_with_missing_causation_id_it_should_skip()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new ConcurrentPocket();
      context.Put(ContextKeys.Broker.MessageMetadata, new Dictionary<string, string>());
      await sut.Execute(context);

      context.TakeOrNull<string>(ContextKeys.Application.CausationId).Should().BeNull("this header not present");
    }

    [Fact]
    public async Task when_executed_with_context_containing_no_message_metadata_it_should_skip()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new ConcurrentPocket();
      await sut.Execute(context);

      context.TakeOrNull<string>(ContextKeys.Application.CorrelationId).Should().BeNull("this header not present");
    }

    [Fact]
    public void when_executed_without_context_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute it's a test
      Func<Task> sut = async () => await new ExtractRelationMetadataStep().Execute(context: null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("context"), "context must be specified");
    }
  }
}
