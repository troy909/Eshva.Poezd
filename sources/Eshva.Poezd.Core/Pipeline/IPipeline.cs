#region Usings

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Eshva.Common.Collections;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  public interface IPipeline
  {
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
    IPipeline Append([JetBrains.Annotations.NotNull] IStep step);

    [JetBrains.Annotations.NotNull]
    Task Execute([JetBrains.Annotations.NotNull] IPocket context);
  }
}
