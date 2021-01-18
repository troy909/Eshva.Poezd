#region Usings

using System;
using Eshva.Poezd.Core.Configuration;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public static class TestDriverExtensions
  {
    public static MessageBrokerConfigurator UseTestDriver(this MessageBrokerConfigurator configurator, TestBrokerDriverConfig config)
    {
      throw new NotImplementedException();
    }
  }

  public class TestBrokerDriverConfig
  {
  }
}
