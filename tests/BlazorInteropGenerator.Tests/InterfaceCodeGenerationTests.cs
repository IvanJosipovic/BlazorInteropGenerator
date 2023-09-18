using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorInteropGenerator.Tests;

public class InterfaceCodeGenerationTests
{
    [Fact]
    public async Task Interface()
    {
        var tsd = """
                /** Interface Comment
                 * line2
                */
                export interface SomeType {
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        @interface.GetLeadingTrivia()[0].ToString().Should().Be("/// <summary>");
        @interface.GetLeadingTrivia()[1].ToString().Should().Be("/// Interface Comment");
        @interface.GetLeadingTrivia()[2].ToString().Should().Be("/// line2");
        @interface.GetLeadingTrivia()[3].ToString().Should().Be("/// </summary>");

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

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);

        Assert.ThrowsAny<NotSupportedException>(() => generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.AnyKeyword, "Test"));
    }

    [Fact]
    public async Task InterfaceMulti()
    {
        var tsd = """
                export interface IType {
                    prop: IType2;
                }

                export interface IType2 {
                    prop: string;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "IType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;
        @interface.Identifier.Text.Should().Be("IType");

        var @interface2 = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[1] as InterfaceDeclarationSyntax;
        @interface2.Identifier.Text.Should().Be("IType2");
    }

    [Fact]
    public async Task InterfaceMultiDuplicate()
    {
        var tsd = """
                export interface IType {
                    prop: IType2;
                    prop2: IType2;
                }

                export interface IType2 {
                    prop: string;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "IType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var ns = syntaxFactory.Members[0] as NamespaceDeclarationSyntax;

        ns.Members.Count.Should().Be(2);

        var @interface = ns.Members[0] as InterfaceDeclarationSyntax;
        @interface.Identifier.Text.Should().Be("IType");

        var @interface2 = ns.Members[1] as InterfaceDeclarationSyntax;
        @interface2.Identifier.Text.Should().Be("IType2");
    }

    [Fact]
    public async Task InterfaceExternal()
    {
        var tsd = """
                import { IType2 } from '@test/tsd2';
                export declare interface IType {
                    prop: IType2;
                }
                """;

        var tsd2 = """
                export declare interface IType2 {
                    prop: string;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        await generator.ParsePackage("tsd2", tsd2);
        var syntaxFactory = generator.GenerateObjects("tsd", "IType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;
        @interface.Identifier.Text.Should().Be("IType");

        var @interface2 = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[1] as InterfaceDeclarationSyntax;
        @interface2.Identifier.Text.Should().Be("IType2");
    }

    [Fact]
    public async Task Extends()
    {
        var tsd = """
                export interface SomeType extends SomeType2 {
                }

                export interface SomeType2 {
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);

        var ns = syntaxFactory.Members[0] as NamespaceDeclarationSyntax;

        var @interface = ns.Members[0] as InterfaceDeclarationSyntax;
        @interface.Identifier.Text.Should().Be("SomeType");


        var @interface2 = ns.Members[1] as InterfaceDeclarationSyntax;
        @interface2.Identifier.Text.Should().Be("SomeType2");
    }

    #region Property

    [Fact]
    public async Task InterfacePropertyComment()
    {
        var tsd = """
                export interface SomeType {
                  /** the comment */
                  prop1: string;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.GetLeadingTrivia()[0].ToString().Should().Be("/// <summary>");
        prop1.GetLeadingTrivia()[1].ToString().Should().Be("/// the comment");
        prop1.GetLeadingTrivia()[2].ToString().Should().Be("/// </summary>");
    }

    [Fact]
    public async Task InterfaceProperty()
    {
        var tsd = """
                export interface SomeType {
                  prop1: string;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Identifier.Text.Should().Be("Prop1");

        prop1.AccessorList.Accessors.Count.Should().Be(2);
        prop1.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
        prop1.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);
    }

    [Fact]
    public async Task InterfacePropertyNullable()
    {
        var tsd = """
                export interface SomeType {
                  prop1?: string;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Type.As<NullableTypeSyntax>().ElementType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");
    }

    [Fact]
    public async Task InterfacePropertyString()
    {
        var tsd = """
                export interface SomeType {
                  prop1: string;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");
    }

    [Fact]
    public async Task InterfacePropertyNumber()
    {
        var tsd = """
                export interface SomeType {
                  prop1: number;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("double");
    }

    [Fact]
    public async Task InterfacePropertyArray()
    {
        var tsd = """
            export interface SomeType {
                prop1: number[];
                prop2: Array<number>;
            }
            """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        (prop1.Type.As<ArrayTypeSyntax>().ElementType as PredefinedTypeSyntax).Keyword.Value.Should().Be("double");

        var prop2 = @interface.Members[1] as PropertyDeclarationSyntax;

        (prop2.Type.As<ArrayTypeSyntax>().ElementType as PredefinedTypeSyntax).Keyword.Value.Should().Be("double");
    }

    [Fact]
    public async Task InterfacePropertyAny()
    {
        var tsd = """
                export interface SomeType {
                  prop1: any;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("object");
    }

    [Fact]
    public async Task InterfacePropertyBoolean()
    {
        var tsd = """
                export interface SomeType {
                  prop1: boolean;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("bool");
    }

    [Fact]
    public async Task InterfacePropertyDictionary()
    {
        var tsd = """
                export interface SomeType {
                  prop1: {[key: string]: any;};
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var prop1 = @interface.Members[0] as PropertyDeclarationSyntax;

        prop1.Type.As<GenericNameSyntax>().Identifier.Value.Should().Be("System.Collections.Generic.Dictionary");
        prop1.Type.As<GenericNameSyntax>().TypeArgumentList.Arguments[0].As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");
        prop1.Type.As<GenericNameSyntax>().TypeArgumentList.Arguments[1].As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("object");
    }

    #endregion

    #region Method

    [Fact]
    public async Task InterfaceMethodComment()
    {
        var tsd = """
                export interface SomeType {
                  /** the comment */
                  method(): void;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.GetLeadingTrivia()[0].ToString().Should().Be("/// <summary>");
        method.GetLeadingTrivia()[1].ToString().Should().Be("/// the comment");
        method.GetLeadingTrivia()[2].ToString().Should().Be("/// </summary>");
    }


    [Fact]
    public async Task InterfaceMethod()
    {
        var tsd = """
                export interface SomeType {
                  method(): void;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.Identifier.Text.Should().Be("Method");
    }

    [Fact]
    public async Task InterfaceMethodVoid()
    {
        var tsd = """
                export interface SomeType {
                  method(): void;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.ReturnType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("void");
    }

    [Fact]
    public async Task InterfaceMethodString()
    {
        var tsd = """
                export interface SomeType {
                  method(): string;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.ReturnType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");
    }

    [Fact]
    public async Task InterfaceMethodStringNullable()
    {
        var tsd = """
                export interface SomeType {
                  method?(): string;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.ReturnType.As<NullableTypeSyntax>().ElementType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");
    }

    [Fact]
    public async Task InterfaceMethodParameter()
    {
        var tsd = """
                export interface SomeType {
                  method(prop1: string): void;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.ParameterList.Parameters.Count.Should().Be(1);
        method.ParameterList.Parameters[0].Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");
        method.ParameterList.Parameters[0].Identifier.Text.Should().Be("prop1");
    }

    [Fact]
    public async Task InterfaceMethodParameterNullable()
    {
        var tsd = """
                export interface SomeType {
                  method(prop1?: string): void;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.ParameterList.Parameters.Count.Should().Be(1);
        method.ParameterList.Parameters[0].Type.As<NullableTypeSyntax>().ElementType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");
    }

    [Fact]
    public async Task InterfaceMethodParameterInvalidName()
    {
        var tsd = """
                export interface SomeType {
                  method(event: string): void;
                }
                """;

        var generator = new Generator();
        await generator.ParsePackage("tsd", tsd);
        var syntaxFactory = generator.GenerateObjects("tsd", "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();

        syntaxFactory.Members.Count.Should().Be(1);
        var @interface = (syntaxFactory.Members[0] as NamespaceDeclarationSyntax).Members[0] as InterfaceDeclarationSyntax;

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.ParameterList.Parameters.Count.Should().Be(1);
        method.ParameterList.Parameters[0].Identifier.Text.Should().Be("@event");
    }


    #endregion
}
