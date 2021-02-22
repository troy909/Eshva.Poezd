#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

#endregion

namespace Eshva.Common.TestTools
{
  /// <summary>
  /// Runtime assembly producer.
  /// </summary>
  public class AssemblyProducer
  {
    /// <summary>
    /// Compiles provided into an assembly.
    /// </summary>
    /// <param name="assemblyName">
    /// Target assembly name.
    /// </param>
    /// <param name="code">
    /// Compiling code.
    /// </param>
    /// <param name="referencedAssemblyLocations">
    /// Locations of assemblies containing types referenced from provided code.
    /// </param>
    /// <returns>
    /// Generated assembly object if compilation was successful.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// One of parameters is null, an empty or a whitespace string.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// An error occurred during compilation or assembly generation.
    /// </exception>
    [NotNull]
    public Assembly MakeAssembly(
      [NotNull] string assemblyName,
      [NotNull] string code,
      [NotNull] IEnumerable<string> referencedAssemblyLocations)
    {
      if (referencedAssemblyLocations == null) throw new ArgumentNullException(nameof(referencedAssemblyLocations));
      if (string.IsNullOrWhiteSpace(code)) throw new ArgumentNullException(nameof(code));
      if (string.IsNullOrWhiteSpace(assemblyName)) throw new ArgumentNullException(nameof(assemblyName));

      return MakeAssembly(
        assemblyName,
        new[] {code},
        referencedAssemblyLocations);
    }

    /// <summary>
    /// Compiles provided into an assembly.
    /// </summary>
    /// <param name="assemblyName">
    /// Target assembly name.
    /// </param>
    /// <param name="code">
    /// Compiling code.
    /// </param>
    /// <param name="referencedAssemblyLocations">
    /// Locations of assemblies containing types referenced from provided code.
    /// </param>
    /// <param name="outputStream">
    /// Stream to write the generated assembly in.
    /// </param>
    /// <returns>
    /// Generated assembly object if compilation was successful.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// One of parameters is null, an empty or a whitespace string.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// An error occurred during compilation or assembly generation.
    /// </exception>
    [NotNull]
    public Assembly MakeAssembly(
      [NotNull] string assemblyName,
      [NotNull] string code,
      [NotNull] IEnumerable<string> referencedAssemblyLocations,
      [NotNull] Stream outputStream)
    {
      return MakeAssembly(
        assemblyName,
        new[] {code},
        referencedAssemblyLocations,
        outputStream);
    }

    /// <summary>
    /// Compiles provided into an assembly.
    /// </summary>
    /// <param name="assemblyName">
    /// Target assembly name.
    /// </param>
    /// <param name="codeParts">
    /// Compiling code parts.
    /// </param>
    /// <param name="referencedAssemblyLocations">
    /// Locations of assemblies containing types referenced from provided code.
    /// </param>
    /// <returns>
    /// Generated assembly object if compilation was successful.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// One of parameters is null, an empty or a whitespace string.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// An error occurred during compilation or assembly generation.
    /// </exception>
    [NotNull]
    public Assembly MakeAssembly(
      [NotNull] string assemblyName,
      [NotNull] IEnumerable<string> codeParts,
      [NotNull] IEnumerable<string> referencedAssemblyLocations)
    {
      using var outputStream = new MemoryStream();
      return MakeAssembly(
        assemblyName,
        codeParts,
        referencedAssemblyLocations,
        outputStream);
    }

    /// <summary>
    /// Compiles provided into an assembly.
    /// </summary>
    /// <param name="assemblyName">
    /// Target assembly name.
    /// </param>
    /// <param name="codeParts">
    /// Compiling code parts.
    /// </param>
    /// <param name="referencedAssemblyLocations">
    /// Locations of assemblies containing types referenced from provided code.
    /// </param>
    /// <param name="outputStream">
    /// Stream to write the generated assembly in.
    /// </param>
    /// <returns>
    /// Generated assembly object if compilation was successful.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// One of parameters is null, an empty or a whitespace string.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// An error occurred during compilation or assembly generation.
    /// </exception>
    [NotNull]
    public Assembly MakeAssembly(
      [NotNull] string assemblyName,
      [NotNull] IEnumerable<string> codeParts,
      [NotNull] IEnumerable<string> referencedAssemblyLocations,
      [NotNull] Stream outputStream)
    {
      if (string.IsNullOrWhiteSpace(assemblyName)) throw new ArgumentNullException(nameof(assemblyName));
      if (codeParts == null) throw new ArgumentNullException(nameof(codeParts));
      if (referencedAssemblyLocations == null) throw new ArgumentNullException(nameof(referencedAssemblyLocations));
      if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
      var codePartsArray = codeParts.ToArray();
      if (!codePartsArray.Any()) throw new ArgumentException("You should provide at least one code part.", nameof(codeParts));

      EmitResult result;
      try
      {
        var compilation = GenerateAssembly(
          assemblyName,
          codePartsArray,
          referencedAssemblyLocations);
        result = compilation.Emit(outputStream);

        if (result.Success)
        {
          outputStream.Seek(offset: 0, SeekOrigin.Begin);
          var assembly = new InternalAssemblyLoadContext().LoadFromStream(outputStream);
          return assembly;
        }
      }
      catch (Exception exception)
      {
        throw new InvalidOperationException(
          "An error occurred during compilation or assembly generation. Inspect inner exception for more details.",
          exception);
      }

      var errorList = new StringBuilder();
      result.Diagnostics.Where(
          diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error).ToList()
        .ForEach(error => errorList.AppendLine($"{error.Id}: {error.GetMessage()}"));
      throw new InvalidOperationException($"Assembly compilation failed. Inspect errors:\n{errorList}");
    }

    private static CSharpCompilation GenerateAssembly(
      string assemblyName,
      IEnumerable<string> codeParts,
      IEnumerable<string> referencedAssemblyLocations)
    {
      var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8);
      var syntaxTrees = codeParts.Select(codePart => SyntaxFactory.ParseSyntaxTree(codePart, options));

      var allReferences = referencedAssemblyLocations
        .Select(location => MetadataReference.CreateFromFile(location))
        .Concat(
          Assembly.GetEntryAssembly()?.GetReferencedAssemblies()
            .Select(name => MetadataReference.CreateFromFile(Assembly.Load(name).Location)) ?? new PortableExecutableReference[] { });

      return CSharpCompilation.Create(
        assemblyName,
        syntaxTrees,
        allReferences,
        new CSharpCompilationOptions(
          OutputKind.DynamicallyLinkedLibrary,
          optimizationLevel: OptimizationLevel.Release,
          assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
    }

    private class InternalAssemblyLoadContext : AssemblyLoadContext
    {
      protected override Assembly Load(AssemblyName assemblyName) => null;
    }
  }
}
