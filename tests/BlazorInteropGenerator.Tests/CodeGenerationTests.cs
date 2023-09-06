using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSDParser.Class;
using Xunit;
using BlazorInteropGenerator;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorInterpGenerator.Tests
{
    public class CodeGenerationTests
    {
        [Fact]
        public void Interface()
        {
            var tsd = TSDParser.TSDParser.ParseDefinition("""
                /* Interface Comment
                 * line2
                */
                export interface SomeType {
                }
                """);

            var syntaxFactory = SyntaxFactory.CompilationUnit();

            syntaxFactory = BlazorInteropGenerator.BlazorInteropGenerator.GenerateInterfaceDeclaration(syntaxFactory, tsd.Statements[0] as InterfaceDeclaration);

            var code = syntaxFactory
               .NormalizeWhitespace()
               .ToFullString();

            syntaxFactory.Members.Count.Should().Be(1);
            var tree = syntaxFactory.Members[0] as InterfaceDeclarationSyntax;

            tree.Modifiers.Count.Should().Be(2);
            tree.Modifiers[0].Value.Should().Be("public");
            tree.Modifiers[1].Value.Should().Be("partial");

            tree.Identifier.Text.Should().Be("SomeType");
        }

        [Fact]
        public void StringProperty()
        {
            var tsd = TSDParser.TSDParser.ParseDefinition("""
                export interface SomeType {
                  prop1: string;
                }
                """);

            var syntaxFactory = SyntaxFactory.CompilationUnit();

            syntaxFactory = BlazorInteropGenerator.BlazorInteropGenerator.GenerateInterfaceDeclaration(syntaxFactory, tsd.Statements[0] as InterfaceDeclaration);

            var code = syntaxFactory
               .NormalizeWhitespace()
               .ToFullString();

            syntaxFactory.Members.Count.Should().Be(1);
            var tree = syntaxFactory.Members[0] as InterfaceDeclarationSyntax;

            var prop = tree.Members[0] as PropertyDeclarationSyntax;

            prop.Modifiers.Count.Should().Be(1);
            prop.Modifiers[0].Value.Should().Be("public");

            prop.Type.As<PredefinedTypeSyntax>().Keyword.Text.Should().Be("string");

            prop.Identifier.Text.Should().Be("prop1");

            prop.AccessorList.Accessors.Count.Should().Be(2);
            prop.AccessorList.Accessors[0].Kind().Should().Be(SyntaxKind.GetAccessorDeclaration);
            prop.AccessorList.Accessors[1].Kind().Should().Be(SyntaxKind.SetAccessorDeclaration);
        }
    }
}
