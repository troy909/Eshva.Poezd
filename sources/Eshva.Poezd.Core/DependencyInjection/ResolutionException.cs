#region Usings

using System;
using System.Runtime.Serialization;

#endregion


namespace Eshva.Poezd.Core.DependencyInjection
{
  /// <summary>
  /// Exceptions that is thrown when something goes wrong while working with the dependency injector.
  /// </summary>
  [Serializable]
  public class ResolutionException : Exception
  {
    public ResolutionException(string message)
      : base(message)
    {
    }

    public ResolutionException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public ResolutionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
