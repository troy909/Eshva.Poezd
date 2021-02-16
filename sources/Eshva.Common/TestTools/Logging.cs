#region Usings

using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.InMemory;
using SimpleInjector;
using Xunit.Abstractions;

#endregion

namespace Eshva.Common.TestTools
{
  public static class Logging
  {
    public static Container AddLogging(this Container container, ITestOutputHelper testOutput)
    {
      container.RegisterInstance(GetLoggerFactory(testOutput));
      container.Register(
        typeof(ILogger<>),
        typeof(Logger<>),
        Lifestyle.Singleton);
      return container;
    }

    private static ILoggerFactory GetLoggerFactory(ITestOutputHelper testOutput) =>
      new LoggerFactory().AddSerilog(
        new LoggerConfiguration()
          .WriteTo.InMemory()
          .WriteTo.TestOutput(testOutput)
          .MinimumLevel.Verbose()
          .CreateLogger());
  }
}
