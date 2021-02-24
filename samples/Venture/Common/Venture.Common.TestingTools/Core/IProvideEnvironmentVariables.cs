#region Usings

using System.Collections.Generic;

#endregion

namespace Venture.Common.TestingTools.Core
{
  public interface IProvideEnvironmentVariables
  {
    IReadOnlyCollection<KeyValuePair<string, string>> GetVariables();
  }
}
