#region Usings

using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  public interface IPipeFitter
  {
    void Setup([NotNull] IPipeline pipeline);
  }
}
