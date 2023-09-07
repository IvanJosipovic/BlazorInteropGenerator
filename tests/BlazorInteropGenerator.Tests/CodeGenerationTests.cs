﻿using Microsoft.CodeAnalysis;
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
    public void ShouldThrowError()
    {
        var tsd = """
                export interface SomeType {
                  method(): string;
                }
                """;

        Assert.Throws<NotSupportedException>(() => Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.AnyKeyword, "Test"));
    }

    #region Property

    [Fact]
    public void InterfacePropertyComment()
    {
        var tsd = """
                export interface SomeType {
                  /* the comment */
                  prop1: string;
                }
                """;

        var syntaxFactory = Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

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
    public void InterfacePropertyAny()
    {
        var tsd = """
                export interface SomeType {
                  prop1: any;
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

        prop1.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("object");

        prop1.Identifier.Text.Should().Be("prop1");

        prop1.AccessorList.Accessors.Count.Should().Be(2);
        prop1.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
        prop1.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);
    }

    [Fact]
    public void InterfacePropertyBoolean()
    {
        var tsd = """
                export interface SomeType {
                  prop1: boolean;
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

        prop1.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("bool");

        prop1.Identifier.Text.Should().Be("prop1");

        prop1.AccessorList.Accessors.Count.Should().Be(2);
        prop1.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
        prop1.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);
    }

    #endregion

    #region Method

    [Fact]
    public void InterfaceMethodComment()
    {
        var tsd = """
                export interface SomeType {
                  /* the comment */
                  method(): void;
                }
                """;

        var syntaxFactory = Generator.GenerateObjects(tsd, "SomeType", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

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

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.Modifiers.Count.Should().Be(1);
        method.Modifiers[0].Value.Should().Be("public");

        method.ReturnType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("void");

        method.Identifier.Text.Should().Be("method");
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

        var method = @interface.Members[0] as MethodDeclarationSyntax;

        method.Modifiers.Count.Should().Be(1);
        method.Modifiers[0].Value.Should().Be("public");

        method.ReturnType.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");

        method.Identifier.Text.Should().Be("method");
    }


    #endregion
}
