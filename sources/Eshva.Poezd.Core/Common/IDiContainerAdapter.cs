#region Usings

using System;

#endregion


namespace Eshva.Poezd.Core.Common
{
  public interface IDiContainerAdapter : IServiceProvider
  {
    IDisposable BeginScope();
  }
}
