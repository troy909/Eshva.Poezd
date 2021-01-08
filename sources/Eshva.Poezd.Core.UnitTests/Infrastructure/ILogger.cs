#region Usings

using System;

#endregion


namespace Eshva.Poezd.Core.UnitTests.Infrastructure
{
  public interface ILogger<T>
  {
    void LogException(Exception exception);
  }
}
