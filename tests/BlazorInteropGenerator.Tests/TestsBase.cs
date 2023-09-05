using BlazorInteropGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace BlazorInterpGenerator.Tests
{
    public class TestsBase
    {

        protected static Compilation CreateCompilation(string source) => CSharpCompilation.Create(
            assemblyName: "compilation",
            syntaxTrees: new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview)) },
            references: Basic.Reference.Assemblies.NetStandard20.References.All.ToArray<MetadataReference>().Append(MetadataReference.CreateFromFile(typeof(BlazorInteropGeneratorAttribute).GetTypeInfo().Assembly.Location)).ToArray(),
            options: new CSharpCompilationOptions(OutputKind.ConsoleApplication)
        );
    }
}