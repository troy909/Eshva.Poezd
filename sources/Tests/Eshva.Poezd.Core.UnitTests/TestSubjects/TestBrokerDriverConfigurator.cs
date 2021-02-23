#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  [UsedImplicitly]
  public sealed class TestBrokerDriverConfigurator
  {
    public TestBrokerDriverConfigurator([NotNull] TestBrokerDriverConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public TestBrokerDriverConfigurator WithSomeSetting(string someSetting)
    {
      _configuration.SomeSettings = someSetting;
      return this;
    }

    private readonly TestBrokerDriverConfiguration _configuration;
  }
}
