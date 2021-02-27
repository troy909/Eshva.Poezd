#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common.Collections;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Message handling pipeline using a linked list.
  /// </summary>
  public sealed class MessageHandlingPipeline : IPipeline<IPocket>
  {
    /// <inheritdoc />
    public IPipeline<IPocket> Append(IStep<IPocket> step)
    {
      if (step == null) throw new ArgumentNullException(nameof(step));

      var stepType = step.GetType();
      if (GetStepOfType(stepType) != null)
        throw new ArgumentException($"A step with type {stepType.FullName} already present in the pipeline.", nameof(step));

      _steps.AddLast(step ?? throw new ArgumentNullException(nameof(step)));

      return this;
    }

    /// <inheritdoc />
    // TODO: Add a cancellation token.
    public async Task Execute(IPocket context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      var currentNode = _steps.First;
      while (currentNode != null)
      {
        var currentStep = currentNode.Value;
        await currentStep.Execute(context);
        currentNode = currentNode.Next;
      }
    }

    private IStep<IPocket> GetStepOfType(Type stepType) => _steps.FirstOrDefault(step => step.GetType() == stepType);

    private readonly LinkedList<IStep<IPocket>> _steps = new();
  }
}
