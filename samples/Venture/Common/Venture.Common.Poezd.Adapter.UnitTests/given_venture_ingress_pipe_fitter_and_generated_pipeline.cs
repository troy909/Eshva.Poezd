#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Venture.Common.Application.MessageHandling;
using Venture.Common.Poezd.Adapter.MessageHandling;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_venture_ingress_pipe_fitter_and_generated_pipeline
  {
    [Fact]
    public void when_append_steps_into_pipeline_it_should_contain_expected_steps_in_expected_order()
    {
      var container = SetupContainer();
      var pipeline = new Pipeline();
      var sut = new VentureIngressPipeFitter(container);

      using (AsyncScopedLifestyle.BeginScope(container))
      {
        sut.AppendStepsInto(pipeline);
      }

      pipeline.Steps.Select(step => step.GetType()).Should().Equal(
        new[]
        {
          typeof(ExtractRelationMetadataStep),
          typeof(ExtractMessageTypeStep),
          typeof(ParseBrokerMessageStep),
          typeof(FindMessageHandlersStep),
          typeof(ExecuteMessageHandlersStep)
        },
        "it is all steps in proper order");
    }

    [Fact]
    public async Task when_append_steps_into_pipeline_it_should_handle_message_expected_way()
    {
      var container = SetupContainer();
      var pipeline = new Pipeline();
      var sut = new VentureIngressPipeFitter(container);

      var context = CreateContext();
      await using (AsyncScopedLifestyle.BeginScope(container))
      {
        sut.AppendStepsInto(pipeline);
        await pipeline.Execute(context);
      }

      context.Message.Should()
        .BeOfType<Message1>()
        .Subject.IsExecuted.Should().BeTrue("message should be handled");
      context.Handlers.Should()
        .NotBeNull("message handler should be found and stored in the context").And
        .BeOfType<HandlerDescriptor[]>()
        .Subject.Single().HandlerType.Should().Be<Massage1Handler>("message handler should be of expected type");
      context.TypeName.Should().Be(nameof(Message1));
      context.CorrelationId.Should().Be("VentureApi.Headers.CorrelationId");
      context.CausationId.Should().Be("VentureApi.Headers.CausationId");
      context.MessageId.Should().Be("VentureApi.Headers.MessageId");
      context.MessageType.Should().Be(typeof(Message1));
      context.PublicApi.Should().NotBeNull();
    }

    [Fact]
    public void when_append_steps_into_pipeline_without_service_provider_it_should_fail()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new VentureIngressPipeFitter(serviceProvider: null);
      sut.Should().Throw<ArgumentNullException>("service provider is required");
    }

    [Fact]
    public async Task when_executed_without_context_it_should_fail()
    {
      var container = SetupContainer();
      var pipeline = new Pipeline();
      var pipeFitter = new VentureIngressPipeFitter(container);

      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once ObjectCreationAsStatement
      Func<Task> sut = () => pipeline.Execute(context: null);
      await using (AsyncScopedLifestyle.BeginScope(container))
      {
        pipeFitter.AppendStepsInto(pipeline);
        sut.Should().Throw<ArgumentNullException>("context is required");
      }
    }

    private static MessageHandlingContext CreateContext()
    {
      var typesRegistry = new IngressMessageTypesRegistry1();
      typesRegistry.Initialize();

      var context = new MessageHandlingContext
      {
        Payload = new byte[1],
        QueueName = "queue name",
        ReceivedOnUtc = DateTimeOffset.UtcNow,
        Metadata = new Dictionary<string, string>
        {
          {VentureApi.Headers.MessageId, "VentureApi.Headers.MessageId"},
          {VentureApi.Headers.CorrelationId, "VentureApi.Headers.CorrelationId"},
          {VentureApi.Headers.CausationId, "VentureApi.Headers.CausationId"},
          {VentureApi.Headers.MessageTypeName, nameof(Message1)}
        },
        PublicApi = new FakePublicApi {MessageTypesRegistry = typesRegistry, HandlerRegistry = new HandlerRegistry()}
      };

      return context;
    }

    private static Container SetupContainer()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.RegisterInstance<IServiceProvider>(container);
      container.RegisterSingleton<IHandlerRegistry, HandlerRegistry>();
      container.RegisterSingleton<IHandlersExecutionStrategy, ExecutionStrategy>();
      container.Register<ExtractRelationMetadataStep>(Lifestyle.Scoped);
      container.Register<ExtractMessageTypeStep>(Lifestyle.Scoped);
      container.Register<ParseBrokerMessageStep>(Lifestyle.Scoped);
      container.Register<FindMessageHandlersStep>(Lifestyle.Scoped);
      container.Register<ExecuteMessageHandlersStep>(Lifestyle.Scoped);
      container.Register<Massage1Handler>(Lifestyle.Scoped);
      return container;
    }

    private class IngressMessageTypesRegistry1 : IngressMessageTypesRegistry
    {
      public override void Initialize()
      {
        var messageType = typeof(Message1);
        AddDescriptor(
          messageType.Name,
          messageType,
          new Descriptor());
      }

      private class Descriptor : IIngressMessageTypeDescriptor<Message1>
      {
        public IReadOnlyCollection<string> QueueNames { get; } = new string[0];

        public Message1 Parse(Memory<byte> bytes) => new Message1();
      }
    }

    private class HandlerRegistry : IHandlerRegistry
    {
      public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; } = new ReadOnlyDictionary<Type, Type[]>(
        new Dictionary<Type, Type[]> {{typeof(Message1), new[] {typeof(Massage1Handler)}}});
    }

    private class Pipeline : IPipeline<MessageHandlingContext>
    {
      public List<IStep<MessageHandlingContext>> Steps { get; } = new List<IStep<MessageHandlingContext>>();

      public IPipeline<MessageHandlingContext> Append(IStep<MessageHandlingContext> step)
      {
        Steps.Add(step);
        return this;
      }

      public async Task Execute(MessageHandlingContext context)
      {
        foreach (var step in Steps)
        {
          await step.Execute(context);
        }
      }
    }

    public class Message1
    {
      public bool IsExecuted { get; set; }
    }

    public class Massage1Handler : IMessageHandler<Message1>
    {
      public Task Handle(Message1 message, VentureIncomingMessageHandlingContext context)
      {
        message.IsExecuted = true;
        return Task.CompletedTask;
      }
    }

    public class ExecutionStrategy : IHandlersExecutionStrategy
    {
      public async Task ExecuteHandlers(
        IEnumerable<HandlerDescriptor> handlers,
        object message,
        VentureIncomingMessageHandlingContext context)
      {
        foreach (var handler in handlers)
        {
          await handler.OnHandle(message, context);
        }
      }
    }
  }
}
