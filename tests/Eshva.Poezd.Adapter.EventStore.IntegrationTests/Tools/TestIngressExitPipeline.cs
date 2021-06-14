#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Adapter.EventStore.IntegrationTests.Tools
{
  internal class TestIngressExitPipeline : IPipeFitter
  {
    public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class
    {
      throw new NotImplementedException();
    }
  }
}
