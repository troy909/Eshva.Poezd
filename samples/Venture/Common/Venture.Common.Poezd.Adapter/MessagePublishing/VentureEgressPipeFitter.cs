#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.Poezd.Adapter.MessagePublishing
{
  public class VentureEgressPipeFitter : TypeBasedLinearPipeFitter
  {
    public VentureEgressPipeFitter([NotNull] IServiceProvider serviceProvider) : base(serviceProvider) { }

    protected override IEnumerable<Type> GetStepTypes()
    {
      yield return typeof(ValidateMessagePublishingContextStep);
      yield return typeof(GetMessageKeyStep);
      yield return typeof(GetTopicNameStep);
      yield return typeof(GetBrokerMetadataStep);
      yield return typeof(SerializeMessageStep);
    }
  }
}
