using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace BlazorInteropGenerator.SourceGenerator
{
    [Generator(LanguageNames.CSharp)]
    public class SourceGenerator : IIncrementalGenerator
    {
        private const string EnumExtensionsAttribute = "BlazorInteropGenerator.BlazorInteropGeneratorAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            //#if DEBUG
            //            if (!Debugger.IsAttached)
            //            {
            //                Debugger.Launch();
            //            }
            //#endif

            //IncrementalValuesProvider<EnumToGenerate?> enumsToGenerate = context.SyntaxProvider
            //    .ForAttributeWithMetadataName(
            //        EnumExtensionsAttribute,
            //        predicate: (node, _) => node is EnumDeclarationSyntax,
            //        transform: GetTypeToGenerate)
            //    .Where(static m => m is not null);

            // find all additional files that end with .txt
            IncrementalValuesProvider<AdditionalText> textFiles = initContext.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".d.ts"));

            // read their contents and save their name
            IncrementalValuesProvider<(string name, string content)> namesAndContents = textFiles.Select((text, cancellationToken) => (name: Path.GetFileName(text.Path), content: text.GetText(cancellationToken)!.ToString()));

            // generate a class that contains their values as const strings
            initContext.RegisterSourceOutput(namesAndContents, (spc, nameAndContent) =>
            {
                spc.AddSource($"ConstStrings.{nameAndContent.name}.cs", $@"//test");
            });
        }
    }
}