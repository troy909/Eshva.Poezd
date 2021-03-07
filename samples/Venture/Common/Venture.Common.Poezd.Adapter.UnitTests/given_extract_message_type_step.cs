#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Venture.CaseOffice.Messages;
using Venture.CaseOffice.Messages.V1.Commands;
using Venture.Common.Poezd.Adapter.MessageHandling;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_extract_message_type_step
  {
    [Fact]
    public async Task when_executed_with_context_containing_message_type_within_broker_message_metadata_it_should_store_it_in_context()
    {
      var registry = new CaseOfficeIngressMessageTypesRegistry();
      registry.Initialize();

      var context = new ConcurrentPocket();
      const string expectedTypeName = "Venture.CaseOffice.Messages.V1.Commands.CreateJusticeCase";
      context.Put(ContextKeys.PublicApi.Itself, new FakePublicApi {MessageTypesRegistry = registry});
      context.Put(
        ContextKeys.Broker.MessageMetadata,
        new Dictionary<string, string> {{VentureApi.Headers.MessageTypeName, expectedTypeName}});
      var sut = new ExtractMessageTypeStep();

      await sut.Execute(context);

      context.TakeOrNull<string>(ContextKeys.Application.MessageTypeName)
        .Should().Be(expectedTypeName, "this header should be copied");
      context.TakeOrNull<Type>(ContextKeys.Application.MessageType)
        .Should().Be(typeof(CreateJusticeCase), "message type should be recognized by its name");
    }

    [Fact]
    public void when_executed_with_context_with_missing_message_type_within_broker_message_metadata_it_should_fail()
    {
      var registry = new CaseOfficeIngressMessageTypesRegistry();
      registry.Initialize();
      var context = new ConcurrentPocket();
      context.Put(ContextKeys.PublicApi.Itself, new FakePublicApi {MessageTypesRegistry = registry});
      context.Put(ContextKeys.Broker.MessageMetadata, new Dictionary<string, string>());
      var step = new ExtractMessageTypeStep();

      Func<Task> sut = () => step.Execute(context);
      sut.Should().Throw<PoezdOperationException>("it's an error to publish message without its type within broker message metadata");
    }

    [Fact]
    public void when_executed_with_context_with_wrong_message_type_within_broker_message_metadata_it_should_fail()
    {
      var registry = new CaseOfficeIngressMessageTypesRegistry();
      registry.Initialize();
      var context = new ConcurrentPocket();
      context.Put(ContextKeys.PublicApi.Itself, new FakePublicApi {MessageTypesRegistry = registry});
      var step = new ExtractMessageTypeStep();

      // ReSharper disable once JoinDeclarationAndInitializer - it's a way to test.
      Func<Task> sut;

      context.Put(
        ContextKeys.Broker.MessageMetadata,
        new Dictionary<string, string> {{VentureApi.Headers.MessageTypeName, null}});
      sut = () => step.Execute(context);
      sut.Should().Throw<PoezdOperationException>("null is a wrong message type value");

      context.Put(
        ContextKeys.Broker.MessageMetadata,
        new Dictionary<string, string> {{VentureApi.Headers.MessageTypeName, string.Empty}});
      sut = () => step.Execute(context);
      sut.Should().Throw<PoezdOperationException>("an empty string is a wrong message type value");

      context.Put(
        ContextKeys.Broker.MessageMetadata,
        new Dictionary<string, string> {{VentureApi.Headers.MessageTypeName, WhitespaceString}});
      sut = () => step.Execute(context);
      sut.Should().Throw<PoezdOperationException>("a whitespace string is a wrong message type value");
    }

    [Fact]
    public void when_executed_with_context_with_unknown_message_type_within_broker_message_metadata_it_should_fail_skip_message_exception()
    {
      var registry = new CaseOfficeIngressMessageTypesRegistry();
      registry.Initialize();
      var step = new ExtractMessageTypeStep();
      var context = new ConcurrentPocket();
      context.Put(ContextKeys.PublicApi.Itself, new FakePublicApi {MessageTypesRegistry = registry});

      context.Put(ContextKeys.Broker.MessageMetadata, new Dictionary<string, string> {{VentureApi.Headers.MessageTypeName, "unknown"}});
      Func<Task> sut = () => step.Execute(context);

      sut.Should().Throw<PoezdOperationException>("unknown type name used as a broker message type name");
    }

    [Fact]
    public void when_executed_with_context_without_message_type_registry_it_should_fail()
    {
      var registry = new CaseOfficeIngressMessageTypesRegistry();
      registry.Initialize();
      var context = new ConcurrentPocket();
      const string expectedTypeName = "Venture.CaseOffice.Messages.V1.Commands.CreateCase";
      context.Put(
        ContextKeys.Broker.MessageMetadata,
        new Dictionary<string, string> {{VentureApi.Headers.MessageTypeName, expectedTypeName}});
      var step = new ExtractMessageTypeStep();

      Func<Task> sut = () => step.Execute(context);
      sut.Should().Throw<KeyNotFoundException>("it can not work without message types registry");
    }

    [Fact]
    public void when_executed_without_context_it_should_fail()
    {
      var registry = new CaseOfficeIngressMessageTypesRegistry();
      registry.Initialize();
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = async () => await new ExtractMessageTypeStep().Execute(context: null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("context"), "context must be specified");
    }

    private const string WhitespaceString = " \t\n\r";
  }
}
