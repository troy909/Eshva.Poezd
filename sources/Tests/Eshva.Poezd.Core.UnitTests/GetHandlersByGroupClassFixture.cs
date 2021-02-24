#region Usings

using System;
using System.IO;
using System.Reflection;
using Eshva.Common.Testing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  [UsedImplicitly]
  public class GetHandlersByGroupClassFixture : IDisposable
  {
    public GetHandlersByGroupClassFixture()
    {
      var assemblyProducer = new AssemblyProducer();
      var interfaceAssemblyName = FileSystem.GenerateRandomFileName(InterfaceAssemblyNamePrefix);
      var interfaceAssemblyPath = FileSystem.CreateTempFilePath(interfaceAssemblyName, "dll");
      using (var interfaceAssemblyStream = File.Create(interfaceAssemblyPath))
      {
        assemblyProducer.MakeAssembly(
          interfaceAssemblyName,
          ReadEmbeddedResource("Eshva.Poezd.Core.UnitTests.TestSubjects.HandlerInterfaces.cs"),
          new[] {typeof(object).Assembly.Location},
          interfaceAssemblyStream);
      }

      var handlers1AssemblyName = FileSystem.GenerateRandomFileName(Handlers1AssemblyNamePrefix);
      var handlers1AssemblyPath = FileSystem.CreateTempFilePath(handlers1AssemblyName, "dll");
      using (var handlers1AssemblyStream = File.Create(handlers1AssemblyPath))
      {
        assemblyProducer.MakeAssembly(
          handlers1AssemblyName,
          ReadEmbeddedResource("Eshva.Poezd.Core.UnitTests.TestSubjects.MessageHandlers1.cs"),
          new[] {typeof(object).Assembly.Location, interfaceAssemblyPath},
          handlers1AssemblyStream);
      }

      var handlers2AssemblyName = FileSystem.GenerateRandomFileName(Handlers2AssemblyNamePrefix);
      var handlers2AssemblyPath = FileSystem.CreateTempFilePath(handlers2AssemblyName, "dll");
      using (var handlers2AssemblyStream = File.Create(handlers2AssemblyPath))
      {
        assemblyProducer.MakeAssembly(
          handlers2AssemblyName,
          ReadEmbeddedResource("Eshva.Poezd.Core.UnitTests.TestSubjects.MessageHandlers2.cs"),
          new[] {typeof(object).Assembly.Location, interfaceAssemblyPath},
          handlers2AssemblyStream);
      }

      var handlers3AssemblyName = FileSystem.GenerateRandomFileName(Handlers3AssemblyNamePrefix);
      var handlers3AssemblyPath = FileSystem.CreateTempFilePath(handlers3AssemblyName, "dll");
      using (var handlers3AssemblyStream = File.Create(handlers3AssemblyPath))
      {
        assemblyProducer.MakeAssembly(
          handlers3AssemblyName,
          ReadEmbeddedResource("Eshva.Poezd.Core.UnitTests.TestSubjects.MessageHandlers3.cs"),
          new[] {typeof(object).Assembly.Location, interfaceAssemblyPath},
          handlers3AssemblyStream);
      }

      InterfaceAssembly = Assembly.LoadFrom(interfaceAssemblyPath);
      Handlers1Assembly = Assembly.LoadFrom(handlers1AssemblyPath);
      Handlers2Assembly = Assembly.LoadFrom(handlers2AssemblyPath);
      Handlers3Assembly = Assembly.LoadFrom(handlers3AssemblyPath);
    }

    public Assembly Handlers3Assembly { get; }

    public Assembly Handlers2Assembly { get; }

    public Assembly Handlers1Assembly { get; }

    public Assembly InterfaceAssembly { get; }

    public void Dispose()
    {
      // IMPORTANT: Unfortunately I can't delete files of assemblies that loaded into the current application domain.
      // The best way to mitigate this problem I can see is by deleting assembly files from prior test runs.
      FileSystem.DeleteIfPossibleTempFilesWithPattern($"{InterfaceAssemblyNamePrefix}*.dll");
      FileSystem.DeleteIfPossibleTempFilesWithPattern($"{Handlers1AssemblyNamePrefix}*.dll");
      FileSystem.DeleteIfPossibleTempFilesWithPattern($"{Handlers2AssemblyNamePrefix}*.dll");
      FileSystem.DeleteIfPossibleTempFilesWithPattern($"{Handlers3AssemblyNamePrefix}*.dll");
    }

    private string ReadEmbeddedResource(string resourceName)
    {
      var resourceStream = GetType().Assembly.GetManifestResourceStream(resourceName);
      using var reader = new StreamReader(resourceStream!);
      var result = reader.ReadToEnd();
      return result;
    }

    private const string InterfaceAssemblyNamePrefix = "InterfaceAssembly";
    private const string Handlers1AssemblyNamePrefix = "Handlers1";
    private const string Handlers2AssemblyNamePrefix = "Handlers2";
    private const string Handlers3AssemblyNamePrefix = "Handlers3";
  }
}
