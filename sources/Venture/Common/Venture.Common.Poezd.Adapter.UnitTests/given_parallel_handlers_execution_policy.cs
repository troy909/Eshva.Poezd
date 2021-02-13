#region Usings

using System.Linq;
using System.Threading.Tasks;
using Eshva.Common.TestTools;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_parallel_handlers_execution_policy
  {
    [Fact]
    public async Task when_executed_with_few_handlers_it_should_execute_all_handlers_in_parallel()
    {
      var sut = new ParallelHandlersExecutionPolicy(NullLogger<ParallelHandlersExecutionPolicy>.Instance);
      var container = Logging.CreateContainerWithLogging();
      // var sut = new ExecuteMessageHandlersStep(
      //   container.GetInstance<ILogger<ExecuteMessageHandlersStep>>(),
      //   new ParallelHandlersExecutionPolicy(container.GetInstance<ILogger<ParallelHandlersExecutionPolicy>>()));

      var handler1 = new MessageHandler();
      var handler2 = new MessageHandler();
      var handler3 = new MessageHandler();
      var handlers = VentureContextTools.CreateHandlerDescriptors(
        handler1,
        handler2,
        handler3).ToArray();
      var message = new Message02();
      var context = VentureContextTools.CreateFilledContext(message, handlers);

      await sut.ExecuteHandlers(
        handlers,
        message,
        context);

      // check results
      handler1.IsExecuted.Should().BeTrue("handler #1 should be called");
      handler2.IsExecuted.Should().BeTrue("handler #2 should be called");
      handler3.IsExecuted.Should().BeTrue("handler #3 should be called");
    }
  }
}
