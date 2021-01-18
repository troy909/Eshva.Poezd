#region Usings

using System;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public sealed class TestBrokerDriverConfigurator : IMessageBrokerDriverConfigurator<TestBrokerDriver>
  {
    public const string TestBrokerSettings = "test broker settings";

    public void Configure([NotNull] TestBrokerDriver driver)
    {
      if (driver == null) throw new ArgumentNullException(nameof(driver));

      driver.SetSomeConfiguration(new TestBrokerDriverConfiguration(TestBrokerSettings));
    }
  }

  public class TestBrokerDriverConfiguration
  {
    public TestBrokerDriverConfiguration(string settings)
    {
      Settings = settings;
    }

    public string Settings { get; }
  }
}
