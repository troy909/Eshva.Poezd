#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Venture.Common.Poezd.Adapter.MessageHandling;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_extract_relation_metadata_step
  {
    [Fact]
    public async Task when_executed_with_context_containing_message_id_within_broker_message_metadata_it_should_store_it_in_context()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new MessageHandlingContext();
      const string expectedId = "expected";
      context.Metadata = new Dictionary<string, string> {{VentureApi.Headers.MessageId, expectedId}};
      await sut.Execute(context);

      context.MessageId.Should().Be(expectedId, "this header should be copied");
    }

    [Fact]
    public async Task when_executed_with_context_containing_message_metadata_with_missing_message_id_it_should_skip()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new MessageHandlingContext {Metadata = new Dictionary<string, string>()};
      await sut.Execute(context);

      context.MessageId.Should().BeNull("this header not present");
    }

    [Fact]
    public async Task when_executed_with_context_containing_correlation_id_within_broker_message_metadata_it_should_store_it_in_context()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new MessageHandlingContext();
      const string expectedCorrelationId = "expected";
      context.Metadata = new Dictionary<string, string> {{VentureApi.Headers.CorrelationId, expectedCorrelationId}};
      await sut.Execute(context);

      context.CorrelationId.Should().Be(expectedCorrelationId, "this header should be copied");
    }

    [Fact]
    public async Task when_executed_with_context_containing_message_metadata_with_missing_correlation_id_it_should_skip()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new MessageHandlingContext {Metadata = new Dictionary<string, string>()};
      await sut.Execute(context);

      context.CorrelationId.Should().BeNull("this header not present");
    }

    [Fact]
    public async Task when_executed_with_context_containing_causation_id_within_broker_message_metadata_it_should_store_it_in_context()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new MessageHandlingContext();
      const string expectedCausationId = "expected";
      context.Metadata = new Dictionary<string, string> {{VentureApi.Headers.CausationId, expectedCausationId}};
      await sut.Execute(context);

      context.CausationId.Should().Be(expectedCausationId, "this header should be copied");
    }

    [Fact]
    public async Task when_executed_with_context_containing_message_metadata_with_missing_causation_id_it_should_skip()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new MessageHandlingContext {Metadata = new Dictionary<string, string>()};
      await sut.Execute(context);

      context.CausationId.Should().BeNull("this header not present");
    }

    [Fact]
    public async Task when_executed_with_context_containing_no_message_metadata_it_should_skip()
    {
      var sut = new ExtractRelationMetadataStep();
      var context = new MessageHandlingContext();
      await sut.Execute(context);

      context.CorrelationId.Should().BeNull("this header not present");
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
