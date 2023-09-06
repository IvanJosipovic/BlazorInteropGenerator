using BlazorInteropGenerator;
using BlazorInteropGenerator.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
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
                [BlazorInteropGeneratorAttribute("test.d.ts")]
                public class Program
                {
                    public static void Main(string[] args)
                    {
                    }
                }
            }
            """;

        string tsd = """
            /* Interface Comment */
            export interface SomeType {
              name: string;
              length: number;
              extras?: string[];
            }
            """;
        var compilation = CreateCompilation(userSource);

        var driver = CSharpGeneratorDriver
            .Create(new IIncrementalGenerator[] { new SourceGenerator() })
            .AddAdditionalTexts(ImmutableArray.CreateRange(new List<AdditionalText>() { new CustomAdditionalText("test.d.ts", tsd) })); ;

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);

        Assert.Empty(diagnostics);
        Assert.Empty(updatedCompilation.GetDiagnostics());
    }
}