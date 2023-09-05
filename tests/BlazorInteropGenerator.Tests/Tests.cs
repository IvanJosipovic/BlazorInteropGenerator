using BlazorInteropGenerator;
using Microsoft;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace BlazorInterpGenerator.Tests;

public class Tests
{
    [Fact]
    public void SimpleGeneratorTest()
    {
        string userSource = """
namespace MyCode
{
    public class Program
    {
        public static void Main(string[] args)
        {
        }
    }
}
""";
        var compilation = CreateCompilation(userSource);

        var driver = CSharpGeneratorDriver.Create(new IIncrementalGenerator[] { new Generator() });
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);

        Assert.Empty(diagnostics);
        Assert.Empty(updatedCompilation.GetDiagnostics());
    }

    private static Compilation CreateCompilation(string source) => CSharpCompilation.Create(
        assemblyName: "compilation",
        syntaxTrees: new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview)) },
        references: new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
        options: new CSharpCompilationOptions(OutputKind.ConsoleApplication)
    );
}