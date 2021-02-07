#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Venture.CaseOffice.Application;
using Venture.Common.Application.MessageHandling;
using Venture.WorkPlanner.Messages.V1.Events;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  public class WorkPlannerHandlersRegistry : IHandlerRegistry
  {
    public WorkPlannerHandlersRegistry()
    {
      var messageHandlersAssemblies = new[] {Assembly.GetAssembly(typeof(AssemblyTag))};

      HandlersGroupedByMessageType = messageHandlersAssemblies
        .GetHandlersGroupedByMessageType(typeof(IHandleMessageOfType<>))
        .Where(pair => pair.Key.FullName!.StartsWith(typeof(TaskCreated).Namespace!))
        .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; }
  }
}
