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
    public async Task InterfaceAsync()
    {
        var tsd = """
                /* Interface Comment
                 * line2
                */
                export interface SomeType {
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        //@interface.GetLeadingTrivia()[0].ToString().Should().Be("/// <summary>");
        //@interface.GetLeadingTrivia()[1].ToString().Should().Be("/// Interface Comment");
        //@interface.GetLeadingTrivia()[2].ToString().Should().Be("/// line2");
        //@interface.GetLeadingTrivia()[3].ToString().Should().Be("/// </summary>");

        @interface.Modifiers.Count.Should().Be(2);
        @interface.Modifiers[0].Value.Should().Be("public");
        @interface.Modifiers[1].Value.Should().Be("partial");

        @interface.Identifier.Text.Should().Be("SomeType");
    }

    [Fact]
    public async Task ShouldThrowError()
    {
        var tsd = """
                export interface SomeType {
                  method(): string;
                }
                """;

        await Assert.ThrowsAsync<NotSupportedException>(async () => await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.AnyKeyword, "Test"));
    }

    #region Property

    [Fact]
    public async Task InterfacePropertyCommentAsync()
    {
        var tsd = """
                export interface SomeType {
                  /* the comment */
                  prop1: string;
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        //prop1.GetLeadingTrivia()[0].ToString().Should().Be("/// <summary>");
        //prop1.GetLeadingTrivia()[1].ToString().Should().Be("/// the comment");
        //prop1.GetLeadingTrivia()[2].ToString().Should().Be("/// </summary>");
    }

    [Fact]
    public async Task InterfacePropertyStringAsync()
    {
        var tsd = """
                export interface SomeType {
                  prop1: string;
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Modifiers.Count.Should().Be(1);
        prop1.Modifiers[0].Value.Should().Be("public");

        prop1.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");

        prop1.Identifier.Text.Should().Be("prop1");

        prop1.AccessorList.Accessors.Count.Should().Be(2);
        prop1.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
        prop1.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);
    }

    [Fact]
    public async Task InterfacePropertyNumberAsync()
    {
        var tsd = """
                export interface SomeType {
                  prop1: number;
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Modifiers.Count.Should().Be(1);
        prop1.Modifiers[0].Value.Should().Be("public");

        prop1.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("double");

        prop1.Identifier.Text.Should().Be("prop1");

        prop1.AccessorList.Accessors.Count.Should().Be(2);
        prop1.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
        prop1.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);
    }

    [Fact]
    public async Task InterfacePropertyArrayAsync()
    {
        var tsd = """
            export interface SomeType {
                prop1: number[];
                prop2: Array<number>;
            }
            """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

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
    public async Task InterfacePropertyAnyAsync()
    {
        var tsd = """
                export interface SomeType {
                  prop1: any;
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Modifiers.Count.Should().Be(1);
        prop1.Modifiers[0].Value.Should().Be("public");

        prop1.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("object");

        prop1.Identifier.Text.Should().Be("prop1");

        prop1.AccessorList.Accessors.Count.Should().Be(2);
        prop1.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
        prop1.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);
    }

    [Fact]
    public async Task InterfacePropertyBooleanAsync()
    {
        var tsd = """
                export interface SomeType {
                  prop1: boolean;
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Modifiers.Count.Should().Be(1);
        prop1.Modifiers[0].Value.Should().Be("public");

        prop1.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("bool");

        prop1.Identifier.Text.Should().Be("prop1");

        prop1.AccessorList.Accessors.Count.Should().Be(2);
        prop1.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
        prop1.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);
    }

    [Fact]
    public async Task InterfacePropertyNullableAsync()
    {
        var tsd = """
                export interface SomeType {
                  prop1?: string;
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Type.As<NullableTypeSyntax>().ElementType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");
    }

    #endregion

    #region Method

    [Fact]
    public async Task InterfaceMethodCommentAsync()
    {
        var tsd = """
                export interface SomeType {
                  /* the comment */
                  method(): void;
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        //method.GetLeadingTrivia()[0].ToString().Should().Be("/// <summary>");
        //method.GetLeadingTrivia()[1].ToString().Should().Be("/// the comment");
        //method.GetLeadingTrivia()[2].ToString().Should().Be("/// </summary>");
    }

    [Fact]
    public async Task InterfaceMethodVoidAsync()
    {
        var tsd = """
                export interface SomeType {
                  method(): void;
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.Modifiers.Count.Should().Be(1);
        method.Modifiers[0].Value.Should().Be("public");

        method.ReturnType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("void");

        method.Identifier.Text.Should().Be("method");
    }

    [Fact]
    public async Task InterfaceMethodStringAsync()
    {
        var tsd = """
                export interface SomeType {
                  method(): string;
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.Modifiers.Count.Should().Be(1);
        method.Modifiers[0].Value.Should().Be("public");

        method.ReturnType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");

        method.Identifier.Text.Should().Be("method");
    }

    [Fact]
    public async Task InterfaceMethodStringNullableAsync()
    {
        var tsd = """
                export interface SomeType {
                  method?(): string;
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.Modifiers.Count.Should().Be(1);
        method.Modifiers[0].Value.Should().Be("public");

        method.ReturnType.As<NullableTypeSyntax>().ElementType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");

        method.Identifier.Text.Should().Be("method");
    }

    [Fact]
    public async Task InterfaceMethodParameterAsync()
    {
        var tsd = """
                export interface SomeType {
                  method(prop1: string): void;
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.ParameterList.Parameters.Count.Should().Be(1);
        method.ParameterList.Parameters[0].Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");
    }

    [Fact]
    public async Task InterfaceMethodParameterNullableAsync()
    {
        var tsd = """
                export interface SomeType {
                  method(prop1?: string): void;
                }
                """;

        var syntaxFactory = await Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.ParameterList.Parameters.Count.Should().Be(1);
        method.ParameterList.Parameters[0].Type.As<NullableTypeSyntax>().ElementType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");
    }

    #endregion
}
