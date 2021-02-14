#region Usings

using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.InMemory;
using SimpleInjector;

#endregion

namespace Eshva.Common.TestTools
{
  public static class Logging
  {
    // TODO: Replace using of this method with AddLogging.
    public static Container CreateContainerWithLogging()
    {
      var container = new Container();
      container.RegisterInstance(GetLoggerFactory());
      container.Register(
        typeof(ILogger<>),
        typeof(Logger<>),
        Lifestyle.Singleton);
      return container;
    }

    public static Container AddLogging(this Container container)
    {
      container.RegisterInstance(GetLoggerFactory());
      container.Register(
        typeof(ILogger<>),
        typeof(Logger<>),
        Lifestyle.Singleton);
      return container;
    }

    private static ILoggerFactory GetLoggerFactory() =>
      new LoggerFactory().AddSerilog(
        new LoggerConfiguration()
          .WriteTo.InMemory()
          .MinimumLevel.Verbose()
          .CreateLogger());
  }
}
