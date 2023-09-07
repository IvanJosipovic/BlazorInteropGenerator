using BlazorInteropGenerator;
using BlazorInteropGenerator.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Reflection;
using Xunit;

namespace BlazorInteropGenerator.Tests.SourceGenerator;

public class GeneratorTests : TestsBase
{
    [Fact]
    public void SimpleGeneratorTest()
    {
        string userSource = """
            using BlazorInteropGenerator;

            namespace MyCode.MyTest
            {
                [BlazorInteropGeneratorAttribute("test.d.ts")]
                public partial interface SomeType {}
            }
            """;

        string tsd = """
            /* Interface Comment */
            export interface SomeType {
              name: string;
              length: number;
            }
            """;
        var compilation = CreateCompilation(userSource);

        var driver = CSharpGeneratorDriver
            .Create(new IIncrementalGenerator[] { new BlazorInteropGenerator.SourceGenerator.SourceGenerator() })
            .AddAdditionalTexts(ImmutableArray.CreateRange(new List<AdditionalText>() { new CustomAdditionalText("test.d.ts", tsd) })); ;

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);

        var @interface = updatedCompilation.SyntaxTrees.Last();

        var code = updatedCompilation.SyntaxTrees.Last().ToString();

        Assert.Empty(diagnostics);
        Assert.Empty(updatedCompilation.GetDiagnostics());
    }
}