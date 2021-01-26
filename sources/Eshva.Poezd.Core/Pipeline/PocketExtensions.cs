#region Usings

using Eshva.Common;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  public static class PocketExtensions
  {
    public static TValue GetContextVariable<TValue>(this IPocket context, string variableName)
    {
      if (!context.TryGet<TValue>(variableName, out var handlers))
        throw new PoezdMessageHandlingException($"Unable to find the '{variableName}' variable in the message handling context.");

      return handlers;
    }
  }
}
