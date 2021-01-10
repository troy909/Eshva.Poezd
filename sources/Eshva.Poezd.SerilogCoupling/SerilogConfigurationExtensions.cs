#region Usings

using Eshva.Poezd.Core.Configuration;

#endregion


namespace Eshva.Poezd.SerilogCoupling
{
  public static class SerilogConfigurationExtensions
  {
    public static LoggingConfigurator UseSerilog(this LoggingConfigurator logging)
    {
      // TODO: Set Serilog as logger.
      return logging;
    }
  }
}
