using BlazorInteropGenerator;
using BlazorInteropGenerator.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using Xunit;

namespace BlazorInterpGenerator.Tests;

public class Tests : TestsBase
{
    [Fact]
    public void SimpleGeneratorTest()
    {
        string userSource = """
using BlazorInteropGenerator;

namespace MyCode
{
    [BlazorInteropGeneratorAttribute("test")]
    public class Program
    {
        public static void Main(string[] args)
        {
        }
    }
}
""";
        var compilation = CreateCompilation(userSource);

        var driver = CSharpGeneratorDriver.Create(new IIncrementalGenerator[] { new SourceGenerator() });
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);

        Assert.Empty(diagnostics);
        Assert.Empty(updatedCompilation.GetDiagnostics());
    }
}