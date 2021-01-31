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
      _properties.Property1 = context.TryTake<string>(Property1, out var property1) ? property1 : string.Empty;
      return Task.CompletedTask;
    }

    private readonly TestProperties _properties;

    public const string Property1 = "property1";
  }
}
