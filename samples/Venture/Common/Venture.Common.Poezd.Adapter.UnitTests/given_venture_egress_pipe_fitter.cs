#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Venture.Common.Poezd.Adapter.Egress;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_venture_egress_pipe_fitter
  {
    [Fact]
    public void when_append_steps_into_pipeline_it_should_contain_expected_steps_in_expected_order()
    {
      var container = SetupContainer();
      var pipeline = new Pipeline();
      var sut = new VentureEgressPipeFitter(container);

      using (AsyncScopedLifestyle.BeginScope(container))
      {
        sut.AppendStepsInto(pipeline);
      }

      pipeline.Steps.Select(step => step.GetType()).Should().Equal(
        new[]
        {
          typeof(ValidateMessagePublishingContextStep),
          typeof(GetMessageKeyStep),
          typeof(GetTopicNameStep),
          typeof(GetBrokerMetadataStep),
          typeof(SerializeMessageStep)
        },
        "it is all steps in proper order");
    }

    [Fact]
    public void when_append_steps_into_pipeline_without_service_provider_it_should_fail()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new VentureEgressPipeFitter(serviceProvider: null);
      sut.Should().Throw<ArgumentNullException>("service provider is required");
    }

    [Fact]
    public async Task when_executed_without_context_it_should_fail()
    {
      var container = SetupContainer();
      var pipeline = new Pipeline();
      var pipeFitter = new VentureEgressPipeFitter(container);

      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once ObjectCreationAsStatement
      Func<Task> sut = () => pipeline.Execute(context: null);
      await using (AsyncScopedLifestyle.BeginScope(container))
      {
        pipeFitter.AppendStepsInto(pipeline);
        sut.Should().Throw<ArgumentNullException>("context is required");
      }
    }

    private static Container SetupContainer()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.RegisterInstance<IServiceProvider>(container);
      container.Register<ValidateMessagePublishingContextStep>(Lifestyle.Scoped);
      container.Register<GetMessageKeyStep>(Lifestyle.Scoped);
      container.Register<GetTopicNameStep>(Lifestyle.Scoped);
      container.Register<GetBrokerMetadataStep>(Lifestyle.Scoped);
      container.Register<SerializeMessageStep>(Lifestyle.Scoped);
      return container;
    }

    private class Pipeline : IPipeline<MessagePublishingContext>
    {
      public List<IStep<MessagePublishingContext>> Steps { get; } = new List<IStep<MessagePublishingContext>>();

      public IPipeline<MessagePublishingContext> Append(IStep<MessagePublishingContext> step)
      {
        Steps.Add(step);
        return this;
      }

      public async Task Execute(MessagePublishingContext context)
      {
        foreach (var step in Steps)
        {
          await step.Execute(context);
        }
      }
    }
  }
}
