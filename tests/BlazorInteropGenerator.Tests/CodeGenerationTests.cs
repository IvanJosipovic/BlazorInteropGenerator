using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TSDParser.Class;
using Xunit;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorInteropGenerator.Tests;

public class CodeGenerationTests
{
    [Fact]
    public void Interface()
    {
        var tsd = """
                /* Interface Comment
                 * line2
                */
                export interface SomeType {
                }
                """;

        var syntaxFactory = Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        @interface.Modifiers.Count.Should().Be(2);
        @interface.Modifiers[0].Value.Should().Be("public");
        @interface.Modifiers[1].Value.Should().Be("partial");

        @interface.Identifier.Text.Should().Be("SomeType");
    }

    [Fact]
    public void InterfacePropertyString()
    {
        var tsd = """
                export interface SomeType {
                  prop1: string;
                }
                """;

        var syntaxFactory = Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop = @interface.Members[0] as PropertyDeclarationSyntax;

        prop.Modifiers.Count.Should().Be(1);
        prop.Modifiers[0].Value.Should().Be("public");

        prop.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");

        prop.Identifier.Text.Should().Be("prop1");

        prop.AccessorList.Accessors.Count.Should().Be(2);
        prop.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
        prop.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);
    }

    [Fact]
    public void InterfacePropertyNumber()
    {
        var tsd = """
                export interface SomeType {
                  prop1: number;
                }
                """;

        var syntaxFactory = Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop = @interface.Members[0] as PropertyDeclarationSyntax;

        prop.Modifiers.Count.Should().Be(1);
        prop.Modifiers[0].Value.Should().Be("public");

        prop.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("double");

        prop.Identifier.Text.Should().Be("prop1");

        prop.AccessorList.Accessors.Count.Should().Be(2);
        prop.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
        prop.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);
    }

    [Fact]
    public void InterfacePropertyArray()
    {
        var tsd = """
            export interface SomeType {
                prop1: number[];
                prop2: Array<number>;
            }
            """;

        var syntaxFactory = Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Modifiers.Count.Should().Be(1);
        prop1.Modifiers[0].Value.Should().Be("public");

        (prop1.Type.As<ArrayTypeSyntax>().ElementType as PredefinedTypeSyntax).Keyword.Value.Should().Be("double");

        prop1.Identifier.Text.Should().Be("prop1");

        prop1.AccessorList.Accessors.Count.Should().Be(2);
        prop1.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
        prop1.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);

        var prop2 = @interface.Members[1] as PropertyDeclarationSyntax;

        prop2.Modifiers.Count.Should().Be(1);
        prop2.Modifiers[0].Value.Should().Be("public");

        (prop2.Type.As<ArrayTypeSyntax>().ElementType as PredefinedTypeSyntax).Keyword.Value.Should().Be("double");

        prop2.Identifier.Text.Should().Be("prop2");

        prop2.AccessorList.Accessors.Count.Should().Be(2);
        prop2.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
        prop2.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);
    }

    [Fact]
    public void InterfaceMethodVoid()
    {
        var tsd = """
                export interface SomeType {
                  method(): void;
                }
                """;

        var syntaxFactory = Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop = @interface.Members[0] as MethodDeclarationSyntax;

        prop.Modifiers.Count.Should().Be(1);
        prop.Modifiers[0].Value.Should().Be("public");

        prop.ReturnType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("void");

        prop.Identifier.Text.Should().Be("method");
    }

    [Fact]
    public void InterfaceMethodString()
    {
        var tsd = """
                export interface SomeType {
                  method(): string;
                }
                """;

        var syntaxFactory = Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop = @interface.Members[0] as MethodDeclarationSyntax;

        prop.Modifiers.Count.Should().Be(1);
        prop.Modifiers[0].Value.Should().Be("public");

        prop.ReturnType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");

        prop.Identifier.Text.Should().Be("method");
    }
}
