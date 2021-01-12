#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public sealed class CustomHandler12 : ICustomHandler<CustomCommand1>, ICustomHandler<CustomCommand2>
  {
    public CustomHandler12(TestProperties properties)
    {
      _properties = properties;
    }

    public Task Handle(CustomCommand1 message, CustomMessageHandlingContext context)
    {
      _properties.Handled1 += 1;
      return Task.CompletedTask;
    }

    public Task Handle(CustomCommand2 message, CustomMessageHandlingContext context)
    {
      _properties.Handled2 += 1;
      return Task.CompletedTask;
    }

    private readonly TestProperties _properties;
  }
}
