#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  public sealed class MessageProcessingPipeline : IPipeline<IStep>
  {
    public IPipeline<IStep> Append([NotNull] IStep step)
    {
      if (step == null)
      {
        throw new ArgumentNullException(nameof(step));
      }

      var stepType = step.GetType();
      if (GetStepOfType(stepType) != null)
      {
        throw new ArgumentException($"A step with type {stepType.FullName} already present in the pipeline.", nameof(step));
      }

      _steps.AddLast(step ?? throw new ArgumentNullException(nameof(step)));

      return this;
    }

    public void InsertBefore([NotNull] IStep step)
    {
      if (step == null)
      {
        throw new ArgumentNullException(nameof(step));
      }

      var stepType = step.GetType();
      var foundStep = GetStepOfType(stepType);
      if (foundStep == null)
      {
        throw new ArgumentException($"Step of type {stepType.FullName} isn't found among pipeline steps.");
      }

      _steps.AddBefore(_steps.Find(foundStep)!, step);
    }

    public void InsertAfter(IStep step)
    {
      if (step == null)
      {
        throw new ArgumentNullException(nameof(step));
      }

      var stepType = step.GetType();
      var foundStep = GetStepOfType(stepType);
      if (foundStep == null)
      {
        throw new ArgumentException($"Step of type {stepType.FullName} isn't found among pipeline steps.");
      }

      _steps.AddAfter(_steps.Find(foundStep)!, step);
    }

    public void Remove(Type stepType)
    {
      var presentStep = GetStepOfType(stepType);
      if (presentStep == null)
      {
        return;
      }

      _steps.Remove(presentStep);
    }

    public async Task Execute(IPocket context)
    {
      var currentNode = _steps.First;
      while (currentNode != null)
      {
        var currentStep = currentNode.Value;
        var nextStep = currentNode.Next?.Value ?? new NoopStep();
        currentNode = currentNode.Next;
        await currentStep.ExecuteWithinContext(context, nextStep);
      }
    }

    private IStep GetStepOfType(Type stepType) => _steps.FirstOrDefault(step => step.GetType() == stepType);

    private readonly LinkedList<IStep> _steps = new LinkedList<IStep>();

    private sealed class NoopStep : IStep
    {
      public Task ExecuteWithinContext(IPocket context, IStep nextStep) => Task.CompletedTask;
    }
  }
}
