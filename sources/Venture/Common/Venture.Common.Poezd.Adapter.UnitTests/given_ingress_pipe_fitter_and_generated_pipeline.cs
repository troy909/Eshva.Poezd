#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Venture.Common.Application.MessageHandling;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_ingress_pipe_fitter_and_generated_pipeline
  {
    [Fact]
    public void when_setup_pipeline_it_should_contain_expected_steps_in_expected_order1()
    {
      var container = SetupContainer();
      var pipeline = new Pipeline();
      var sut = new VentureIngressPipeFitter(container);

      using (AsyncScopedLifestyle.BeginScope(container))
      {
        sut.Setup(pipeline);
      }

      pipeline.Steps.Select(step => step.GetType()).Should().Equal(
        new[]
        {
          typeof(ExtractRelationMetadataStep),
          typeof(ExtractMessageTypeStep),
          typeof(ParseBrokerMessageStep),
          typeof(ExtractAuthorizationMetadataStep),
          typeof(FindMessageHandlersStep),
          typeof(ExecuteMessageHandlersStep)
        },
        "it is all steps in proper order");
    }

    [Fact]
    public async Task when_setup_pipeline_it_should_handle_message_expected_way()
    {
      var container = SetupContainer();
      var pipeline = new Pipeline();
      var sut = new VentureIngressPipeFitter(container);

      var context = CreateContext();
      await using (AsyncScopedLifestyle.BeginScope(container))
      {
        sut.Setup(pipeline);
        await pipeline.Execute(context);
      }

      context.TakeOrThrow<Message1>(ContextKeys.Application.MessagePayload).IsExecuted.Should().BeTrue("message should be handled");
      context.TakeOrThrow<IEnumerable<HandlerDescriptor>>(ContextKeys.Application.Handlers).Should()
        .NotBeNull("message handler should be found and stored in the context").And
        .Subject.Single().HandlerType.Should().Be<Massage1Handler>("message handler should be of expected type");
      context.TakeOrThrow<string>(ContextKeys.Application.MessageTypeName).Should().Be(nameof(Message1));
      context.TakeOrThrow<string>(ContextKeys.Application.MessageId).Should().Be("VentureApi.Headers.MessageId");
      context.TakeOrThrow<string>(ContextKeys.Application.CorrelationId).Should().Be("VentureApi.Headers.CorrelationId");
      context.TakeOrThrow<string>(ContextKeys.Application.CausationId).Should().Be("VentureApi.Headers.CausationId");
      context.TakeOrThrow<Type>(ContextKeys.Application.MessageType).Should().Be(typeof(Message1));
      context.TakeOrThrow<MessageTypesRegistry1>(ContextKeys.PublicApi.MessageTypesRegistry).Should().NotBeNull();
    }

    [Fact]
    public void when_setup_pipeline_without_service_provider_it_should_fail()
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
        pipeFitter.Setup(pipeline);
        sut.Should().Throw<ArgumentNullException>("context is required");
      }
    }

    private static ConcurrentPocket CreateContext()
    {
      var context = new ConcurrentPocket();
      context.Put(ContextKeys.Broker.MessagePayload, new byte[1]);
      context.Put(ContextKeys.Broker.QueueName, "queue name");
      context.Put(
        ContextKeys.Broker.MessageMetadata,
        new Dictionary<string, string>
        {
          {VentureApi.Headers.MessageId, "VentureApi.Headers.MessageId"},
          {VentureApi.Headers.CorrelationId, "VentureApi.Headers.CorrelationId"},
          {VentureApi.Headers.CausationId, "VentureApi.Headers.CausationId"},
          {VentureApi.Headers.MessageTypeName, nameof(Message1)}
        });
      var typesRegistry = new MessageTypesRegistry1();
      typesRegistry.Initialize();
      context.Put(ContextKeys.PublicApi.MessageTypesRegistry, typesRegistry);
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
      container.Register<ExtractAuthorizationMetadataStep>(Lifestyle.Scoped);
      container.Register<FindMessageHandlersStep>(Lifestyle.Scoped);
      container.Register<ExecuteMessageHandlersStep>(Lifestyle.Scoped);
      container.Register<Massage1Handler>(Lifestyle.Scoped);
      return container;
    }

    private class MessageTypesRegistry1 : MessageTypesRegistry
    {
      public override void Initialize()
      {
        var messageType = typeof(Message1);
        AddDescriptor(
          messageType.Name,
          messageType,
          new Descriptor());
      }

      private class Descriptor : IMessageTypeDescriptor<Message1>
      {
        public Message1 Parse(Memory<byte> bytes) => new Message1();

        // ReSharper disable once RedundantAssignment
        public int Serialize(Message1 message, Span<byte> buffer)
        {
          // ReSharper disable once RedundantAssignment
          buffer = new byte[1];
          return 1;
        }
      }
    }

    private class HandlerRegistry : IHandlerRegistry
    {
      public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; } = new ReadOnlyDictionary<Type, Type[]>(
        new Dictionary<Type, Type[]> {{typeof(Message1), new[] {typeof(Massage1Handler)}}});
    }

    private class Pipeline : IPipeline
    {
      public List<IStep> Steps { get; } = new List<IStep>();

      public IPipeline Append(IStep step)
      {
        Steps.Add(step);
        return this;
      }

      public async Task Execute(IPocket context)
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

    public class Massage1Handler : IHandleMessageOfType<Message1>
    {
      public Task Handle(Message1 message, VentureContext context)
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
        VentureContext context)
      {
        foreach (var handler in handlers)
        {
          await handler.OnHandle(message, context);
        }
      }
    }
  }
}
