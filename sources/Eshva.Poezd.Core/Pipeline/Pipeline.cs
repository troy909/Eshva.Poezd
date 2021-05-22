#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Common;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Message processing pipeline which using a linked list.
  /// </summary>
  public sealed class Pipeline<TContext> : IPipeline<TContext> where TContext : class, IPocket, ICanSkipFurtherMessageHandling
  {
    /// <inheritdoc />
    public IPipeline<TContext> Append(IStep<TContext> step)
    {
      if (step == null) throw new ArgumentNullException(nameof(step));

      var stepType = step.GetType();
      if (GetStepOfType(stepType) != null)
        throw new ArgumentException($"A step with type {stepType.FullName} already present in the pipeline.", nameof(step));

      _steps.AddLast(step);

      return this;
    }

    /// <inheritdoc />
    public async Task Execute(TContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      var currentNode = _steps.First;
      while (currentNode != null)
      {
        var currentStep = currentNode.Value;
        try
        {
          await currentStep.Execute(context);
          if (context.ShouldSkipFurtherMessageHandling) break;
        }
        catch (Exception exception)
        {
          throw new PoezdOperationException(
            $"An error occurred during executing a pipeline step of type {currentNode.Value.GetType().FullName}.",
            exception);
        }

        currentNode = currentNode.Next;
      }
    }

    private IStep<TContext> GetStepOfType(Type stepType) => _steps.FirstOrDefault(step => step.GetType() == stepType);

    private readonly LinkedList<IStep<TContext>> _steps = new LinkedList<IStep<TContext>>();
  }
}
