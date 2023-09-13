using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TSDParser.Class;
using Xunit;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorInteropGenerator.Tests;

public class ClassCodeGenerationTests
{
    [Fact]
    public async Task Class()
    {
        var tsd = """
                /** Class Comment
                 * line2
                */
                export class SomeType {
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.ClassDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @class = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as ClassDeclarationSyntax;

        @class.GetLeadingTrivia()[0].ToString().Should().Be("/// <summary>");
        @class.GetLeadingTrivia()[1].ToString().Should().Be("/// Class Comment");
        @class.GetLeadingTrivia()[2].ToString().Should().Be("/// line2");
        @class.GetLeadingTrivia()[3].ToString().Should().Be("/// </summary>");

        @class.Modifiers.Count.Should().Be(2);
        @class.Modifiers[0].Value.Should().Be("public");
        @class.Modifiers[1].Value.Should().Be("partial");

        @class.Identifier.Text.Should().Be("SomeType");
    }

    [Fact]
    public async Task ClassPropertyComment()
    {
        var tsd = """
                export class SomeType {
                  /** the comment */
                  prop1: string;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.ClassDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @class = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as ClassDeclarationSyntax;

        var prop1 = @class.Members[0] as PropertyDeclarationSyntax;

        prop1.GetLeadingTrivia()[0].ToString().Should().Be("/// <summary>");
        prop1.GetLeadingTrivia()[1].ToString().Should().Be("/// the comment");
        prop1.GetLeadingTrivia()[2].ToString().Should().Be("/// </summary>");
    }

    [Fact]
    public async Task ClassProperty()
    {
        var tsd = """
                export class SomeType {
                  prop1: string;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.ClassDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @class = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as ClassDeclarationSyntax;

        var prop1 = @class.Members[0] as PropertyDeclarationSyntax;

        prop1.Modifiers.Count.Should().Be(1);
        prop1.Modifiers[0].Value.Should().Be("public");

        prop1.Identifier.Text.Should().Be("Prop1");

        prop1.AccessorList.Accessors.Count.Should().Be(2);
        prop1.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
        prop1.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);
    }

    [Fact]
    public async Task ClassPropertyString()
    {
        var tsd = """
                export class SomeType {
                  prop1: string;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.ClassDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @class = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as ClassDeclarationSyntax;

        var prop1 = @class.Members[0] as PropertyDeclarationSyntax;

        prop1.Modifiers.Count.Should().Be(1);
        prop1.Modifiers[0].Value.Should().Be("public");

        prop1.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");
    }

    [Fact]
    public async Task ClassMethodVoid()
    {
        var tsd = """
                export class SomeType {
                  method(): void;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.ClassDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @class = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as ClassDeclarationSyntax;

        var method = @class.Members[0] as MethodDeclarationSyntax;

        method.Modifiers.Count.Should().Be(1);
        method.Modifiers[0].Value.Should().Be("public");

        method.ReturnType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("void");

        method.Identifier.Text.Should().Be("Method");
    }
}
