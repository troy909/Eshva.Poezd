#region Usings

using System.Collections.Generic;

#endregion

namespace Venture.Common.TestingTools
{
  public interface IProvideEnvironmentVariables
  {
    IReadOnlyCollection<KeyValuePair<string, string>> GetVariables();
  }
}
