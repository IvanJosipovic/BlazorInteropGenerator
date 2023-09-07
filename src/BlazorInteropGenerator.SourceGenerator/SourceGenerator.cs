using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using TSDParser.Enums;

namespace BlazorInteropGenerator.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class SourceGenerator : IIncrementalGenerator
{
    private const string EnumExtensionsAttribute = "BlazorInteropGenerator.BlazorInteropGeneratorAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext initContext)
    {
#if DEBUG
        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}
#endif

        // find all additional files that end with .d.ts
        IncrementalValuesProvider<AdditionalText> tsDefinitions = initContext.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".d.ts"));

        // read their contents and save their name
        IncrementalValuesProvider<TSD> namesAndContents = tsDefinitions.Select((text, cancellationToken) => new TSD() { Name = Path.GetFileName(text.Path), Content = text.GetText(cancellationToken)!.ToString()});

        IncrementalValuesProvider<ObjectToGenerate?> objectsToGenerate = initContext.SyntaxProvider
            .ForAttributeWithMetadataName(
                EnumExtensionsAttribute,
                predicate: (node, _) => node is InterfaceDeclarationSyntax or ClassDeclarationSyntax,
                transform: GetTypeToGenerate)
            .Where(static m => m is not null);

        var combined = objectsToGenerate.Combine(namesAndContents.Collect());

        // generate a class that contains their values as const strings
        initContext.RegisterSourceOutput(combined, (spc, combined) =>
        {
            var syntax = Generator.GenerateObjects(combined.Right.First(x => x.Name == combined.Left.Name).Content, combined.Left.ObjectName, combined.Left.SyntaxKind.Value, combined.Left.Namespace);

            var code = syntax
                .NormalizeWhitespace()
                .ToFullString();

            spc.AddSource($"Generated.{combined.Left.ObjectName}.cs", code);
        });
    }

    static ObjectToGenerate? GetTypeToGenerate(GeneratorAttributeSyntaxContext context, CancellationToken ct)
    {
        var attr = context.Attributes.Where(x => x.AttributeClass.Name == "BlazorInteropGeneratorAttribute").First();

        var response = new ObjectToGenerate()
        {
            Name = attr.ConstructorArguments[0].Value.ToString(),
            ObjectName = attr.ConstructorArguments[1].Value.ToString(),
            Namespace = context.TargetSymbol.ContainingNamespace.ToString(),
            SyntaxKind = context.TargetNode.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.InterfaceDeclaration) ? SyntaxKind.InterfaceDeclaration : SyntaxKind.ClassDeclaration,
        };

        return response;
    }
}

public class TSD
{
    public string Name { get; set; }

    public string Content { get; set; }
}

public class ObjectToGenerate
{
    /// <summary>
    /// TSD Name or NPM Project Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Object to Generate
    /// </summary>
    public string? ObjectName { get; set; }

    /// <summary>
    /// Syntax Kind for Object Name
    /// </summary>
    public SyntaxKind? SyntaxKind { get; set; }

    /// <summary>
    /// Namespace to generate code in
    /// </summary>
    public string Namespace { get; set; }
}