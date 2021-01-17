#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  /// <summary>
  /// The imaginary sample application cares only about number of all change data capture events.
  /// So we don't need to deserialize message payload.
  /// </summary>
  [UsedImplicitly]
  public sealed class CdcNotificationsPipelineConfigurator : TypeBasedLinearPipelineConfigurator
  {
    public CdcNotificationsPipelineConfigurator([NotNull] IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    protected override IEnumerable<Type> GetStepTypes()
    {
      yield return typeof(CommitMessageStep);
      yield return typeof(CdcNotificationsCommitStep);
    }
  }
}
