#region Usings

using System.Reflection;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Activation;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using FluentAssertions;
using SimpleInjector;
using Xunit;

#endregion


namespace Eshva.Poezd.Core.UnitTests
{
  public sealed class given_message_handler_factory
  {
    [Fact]
    public async Task when_handlers_requested_it_should_return_all_registered_application_handlers_adopted()
    {
      var container = new Container();
      var testProperties = new TestProperties();
      container.RegisterInstance(testProperties);
      container.Collection.Register(typeof(ICustomHandler<>), Assembly.GetExecutingAssembly());
      container.Verify();

      var sut = new CustomMessageHandlerFactory(container);
      var handlers = await sut.GetHandlersOfMessage(new CustomCommand1(), new TransactionContext());
      handlers.Should()
              .HaveCount(2, $"there is 2 handlers of {nameof(CustomCommand1)}: {nameof(CustomHandler1)} and {nameof(CustomHandler2)}");
    }
  }
}
