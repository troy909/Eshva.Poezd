#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public sealed class CustomHandler23 : ICustomHandler<CustomCommand2>, ICustomHandler<CustomCommand3>
  {
    public CustomHandler23(TestProperties properties)
    {
      _properties = properties;
    }

    public Task Handle(CustomCommand2 message, CustomMessageHandlingContext context)
    {
      _properties.Handled2 += 1;
      return Task.CompletedTask;
    }

    public Task Handle(CustomCommand3 message, CustomMessageHandlingContext context)
    {
      _properties.Handled3 += 1;
      return Task.CompletedTask;
    }

    private readonly TestProperties _properties;
  }
}
