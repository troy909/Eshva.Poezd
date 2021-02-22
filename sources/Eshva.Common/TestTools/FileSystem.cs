#region Usings

using System;
using System.IO;

#endregion

namespace Eshva.Common.TestTools
{
  public class FileSystem
  {
    public static string CreateTempFilePath(string interfaceAssemblyName, string extension) =>
      Path.Combine(Path.GetTempPath(), $"{interfaceAssemblyName}.{extension}");

    public static string GenerateRandomFileName(string fixedPart) =>
      $"{fixedPart}-{new Random().Next():D10}";

    public static void DeleteIfPossibleTempFilesWithPattern(string fileNamePattern)
    {
      var foundFiles = Directory.GetFiles(Path.GetTempPath(), fileNamePattern);
      foreach (var filePath in foundFiles)
      {
        try
        {
          File.Delete(filePath);
        }
        catch (Exception)
        {
          // Intentionally ignore any errors. We did our best.
        }
      }
    }
  }
}
