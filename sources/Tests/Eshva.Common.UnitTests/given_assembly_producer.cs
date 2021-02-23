#region Usings

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Eshva.Common.TestTools;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Common.UnitTests
{
  public class given_assembly_producer_making_assembly
  {
    [Fact]
    public void when_provided_single_correct_code_and_references_it_should_produce_assembly()
    {
      var sut = new AssemblyProducer();
      const string testEventClassNamespace = "Test";
      const string testEventClassName = "Event";
      var code =
        @$"using System; namespace {testEventClassNamespace} {{ public class {testEventClassName} {{ public Guid Id {{get; set;}} }} }}";
      var references = new[] {typeof(object).Assembly.Location};
      const string assemblyName = "TestAssembly";

      var assembly = sut.MakeAssembly(
        assemblyName,
        code,
        references);

      assembly.GetName().Name.Should().Be(assemblyName, "assembly should have expected name");
      var testEvent = assembly.CreateInstance($"{testEventClassNamespace}.{testEventClassName}");
      testEvent.Should().NotBeNull("object of type defined in generated assembly should be created");
      testEvent!.GetType().Name.Should().Be(testEventClassName, "instantiated object type should have expected name");
      testEvent!.GetType().Namespace.Should().Be(testEventClassNamespace, "instantiated object type should have expected namespace");
    }

    [Fact]
    public void when_provided_few_correct_code_strings_and_references_it_should_produce_assembly()
    {
      var sut = new AssemblyProducer();
      const string testEventClassNamespace = "Test";
      const string testEvent1ClassName = "Event1";
      const string testEvent2ClassName = "Event2";
      var code1 =
        @$"using System; namespace {testEventClassNamespace} {{ public class {testEvent1ClassName} {{ public Guid Id {{get; set;}} }} }}";
      var code2 =
        @$"using System; namespace {testEventClassNamespace} {{ public class {testEvent2ClassName} {{ public Guid Id {{get; set;}} }} }}";
      var references = new[] {typeof(object).Assembly.Location};
      const string assemblyName = "TestAssembly";

      var assembly = sut.MakeAssembly(
        assemblyName,
        new[] {code1, code2},
        references);

      assembly.GetName().Name.Should().Be(assemblyName, "assembly should have expected name");
      var testEvent1 = assembly.CreateInstance($"{testEventClassNamespace}.{testEvent1ClassName}");
      testEvent1.Should().NotBeNull("object of type defined in generated assembly should be created");
      testEvent1!.GetType().Name.Should().Be(testEvent1ClassName, "instantiated object type should have expected name");
      testEvent1!.GetType().Namespace.Should().Be(testEventClassNamespace, "instantiated object type should have expected namespace");
      var testEvent2 = assembly.CreateInstance($"{testEventClassNamespace}.{testEvent2ClassName}");
      testEvent2.Should().NotBeNull("object of type defined in generated assembly should be created");
      testEvent2!.GetType().Name.Should().Be(testEvent2ClassName, "instantiated object type should have expected name");
      testEvent2!.GetType().Namespace.Should().Be(testEventClassNamespace, "instantiated object type should have expected namespace");
    }

    [Fact]
    public void when_required_to_reference_another_self_made_assembly_it_should_be_possible()
    {
      var sut = new AssemblyProducer();
      const string myNamespace = "Test";
      const string interfaceName = "IMyInterface";
      const string className = "MyClassName";
      var interfaceAssemblyCode =
        @$"using System; namespace {myNamespace} {{ public interface {interfaceName} {{ }} }}";
      var classAssemblyCode =
        @$"using System; namespace {myNamespace} {{ public class {className} : {interfaceName} {{ }} }}";
      var references = new[] {typeof(object).Assembly.Location};

      var interfaceAssemblyName = FileSystem.GenerateRandomFileName("InterfaceAssembly");
      var interfaceAssemblyPath = FileSystem.CreateTempFilePath(interfaceAssemblyName, "dll");
      Assembly interfaceAssembly;
      using (var interfaceAssemblyStream = File.Create(interfaceAssemblyPath))
      {
        interfaceAssembly = sut.MakeAssembly(
          interfaceAssemblyName,
          new[] {interfaceAssemblyCode},
          references,
          interfaceAssemblyStream);
      }

      interfaceAssembly.ExportedTypes.Select(type => type.FullName)
        .Should().Contain($"{myNamespace}.{interfaceName}", "interface type should be found");

      var classAssemblyName = FileSystem.GenerateRandomFileName("ClassAssembly");
      var classAssemblyPath = FileSystem.CreateTempFilePath(classAssemblyName, "dll");
      using (var classAssemblyStream = File.Create(classAssemblyPath))
      {
        sut.MakeAssembly(
          classAssemblyName,
          new[] {classAssemblyCode},
          references.Concat(new[] {interfaceAssemblyPath}),
          classAssemblyStream);
      }

      var classAssembly = Assembly.LoadFrom(classAssemblyPath);
      classAssembly.ExportedTypes.Select(type => type.FullName)
        .Should().Contain($"{myNamespace}.{className}", "class type should be found");

      // IMPORTANT: Unfortunately I can't delete files of assemblies that loaded into the current application domain.
      // The best way to mitigate this problem I can see is by deleting assembly files from prior test runs.
      FileSystem.DeleteIfPossibleTempFilesWithPattern($"{interfaceAssemblyName}*.dll");
      FileSystem.DeleteIfPossibleTempFilesWithPattern($"{classAssemblyName}*.dll");
    }

    [Fact]
    public void when_provided_code_with_errors_it_should_fail_with_list_of_errors()
    {
      var assemblyProducer = new AssemblyProducer();
      const string code = @"using System; namespace Test {{ public class Event { public UnknownType Id {get; set;} } }";
      var references = new[] {typeof(object).Assembly.Location, typeof(Console).Assembly.Location};
      const string assemblyName = "TestAssembly";

      Action sut = () => assemblyProducer.MakeAssembly(
        assemblyName,
        code,
        references);

      sut.Should().Throw<InvalidOperationException>("the code provided isn't compilable")
        .Where(exception => exception.Message.Contains("CS0246"), "exception message should contain expected error");
    }

    [Fact]
    public void when_code_is_not_specified_it_should_fail()
    {
      var assemblyProducer = new AssemblyProducer();
      var references = new[] {typeof(object).Assembly.Location, typeof(Console).Assembly.Location};
      const string assemblyName = "TestAssembly";

      string code = null;
      // ReSharper disable once AccessToModifiedClosure - it's a way to test.
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => assemblyProducer.MakeAssembly(
        assemblyName,
        code,
        references);

      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("code"));
      code = string.Empty;
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("code"));
      code = WhitespaceString;
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("code"));
    }

    [Fact]
    public void when_assembly_name_is_not_specified_it_should_fail()
    {
      var assemblyProducer = new AssemblyProducer();
      const string code = @"using System; namespace Test {{ public class Event { public Guid Id {get; set;} } }";
      var references = new[] {typeof(object).Assembly.Location, typeof(Console).Assembly.Location};

      string assemblyName = null;
      // ReSharper disable once AccessToModifiedClosure - it's a way to test.
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => assemblyProducer.MakeAssembly(
        assemblyName,
        code,
        references);

      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("assemblyName"));
      assemblyName = string.Empty;
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("assemblyName"));
      assemblyName = WhitespaceString;
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("assemblyName"));
    }

    [Fact]
    public void when_references_not_specified_it_should_fail()
    {
      var assemblyProducer = new AssemblyProducer();
      const string code = @"using System; namespace Test {{ public class Event { public Guid Id {get; set;} } }";
      const string assemblyName = "TestAssembly";

      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => assemblyProducer.MakeAssembly(
        assemblyName,
        code,
        referencedAssemblyLocations: null);

      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("referencedAssemblyLocations"));
    }

    private const string WhitespaceString = " \t\n";
  }
}
