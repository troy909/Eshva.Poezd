#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public sealed class CustomHandler1 : ICustomHandler<CustomCommand1>
  {
    public CustomHandler1(TestProperties properties)
    {
      _properties = properties;
    }

    public Task Handle(CustomCommand1 message, CustomMessageHandlingContext context)
    {
      _properties.Handled1 += 1;
      return Task.CompletedTask;
    }

    private readonly TestProperties _properties;
  }
}
