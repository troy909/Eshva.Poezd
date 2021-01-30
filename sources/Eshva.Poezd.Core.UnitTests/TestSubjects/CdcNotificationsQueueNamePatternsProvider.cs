#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class CdcNotificationsQueueNamePatternsProvider : IQueueNamePatternsProvider
  {
    public IEnumerable<string> GetQueueNamePatterns()
    {
      yield return @"^sample\.cdc\..*";
    }
  }
}
