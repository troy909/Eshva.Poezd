#region Usings

using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.InMemory;
using SimpleInjector;

#endregion

namespace Eshva.Common.TestTools
{
  public class Logging
  {
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

    private static ILoggerFactory GetLoggerFactory() =>
      new LoggerFactory().AddSerilog(
        new LoggerConfiguration()
          .WriteTo.InMemory()
          .MinimumLevel.Verbose()
          .CreateLogger());
  }
}
