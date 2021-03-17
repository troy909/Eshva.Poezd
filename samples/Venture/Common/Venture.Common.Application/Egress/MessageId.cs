#region Usings

using System;
using System.Runtime.CompilerServices;

#endregion

namespace Venture.Common.Application.Egress
{
  /// <summary>
  /// Tools for message ID.
  /// </summary>
  public static class MessageId
  {
    /// <summary>
    /// Generates new random message ID as a string.
    /// </summary>
    /// <remarks>
    /// It generates ID from a new GUID formatted with 'digits' format.
    /// </remarks>
    /// <returns>
    /// A new random message ID.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Generate() => Format(Guid.NewGuid());

    /// <summary>
    /// Converts message ID into a string.
    /// </summary>
    /// <param name="id">
    /// GUID to convert into string.
    /// </param>
    /// <returns>
    /// A string version of the message ID.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format(Guid id) => id.ToString(ExpectedFormat);

    private const string ExpectedFormat = "N";
  }
}
