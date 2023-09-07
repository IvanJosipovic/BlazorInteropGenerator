using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TSDParser.Class;
using TSDParser.Enums;
using TSDParser.Interfaces;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;
using TSSyntaxKind = TSDParser.Enums.SyntaxKind;

namespace BlazorInteropGenerator;

/// <summary>
/// Generates C# Code from TypeScript Definitions
/// </summary>
public static class Generator
{
    public static CompilationUnitSyntax GenerateObjects(string typeScriptDefinition, string objectName, TSSyntaxKind syntaxKind, string @namespace)
    {
        var syntaxFactory = SyntaxFactory.CompilationUnit();

        var tsd = TSDParser.TSDParser.ParseDefinition(typeScriptDefinition);

        var @namespaceObj = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(@namespace)).NormalizeWhitespace();

        if (syntaxKind == TSSyntaxKind.InterfaceDeclaration)
        {
            var interfaceDeclaration = tsd.Statements.Where(x => x.Kind == syntaxKind).Cast<InterfaceDeclaration>().First(x => x.Name.Text == objectName);

            namespaceObj = namespaceObj.AddMembers(GenerateInterfaceDeclaration(syntaxFactory, interfaceDeclaration).Members.ToArray());
        }
        else if (syntaxKind == TSSyntaxKind.ClassDeclaration)
        {
            var classDeclaration = tsd.Statements.Where(x => x.Kind == syntaxKind).Cast<ClassDeclaration>().First(x => x.Name.Text == objectName);
        }
        else
        {
            throw new NotSupportedException();
        }

        syntaxFactory = syntaxFactory.AddMembers(namespaceObj);

        return syntaxFactory;
    }

    public static CompilationUnitSyntax GenerateInterfaceDeclaration(CompilationUnitSyntax syntaxFactory, InterfaceDeclaration interfaceDeclaration)
    {
        var @interface = SyntaxFactory.InterfaceDeclaration(interfaceDeclaration.Name.Text);

        @interface = @interface.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        @interface = @interface.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

        if (interfaceDeclaration.JSDoc != null)
        {
            @interface = AddComment(@interface, interfaceDeclaration.JSDoc.Comment) as InterfaceDeclarationSyntax;
        }

        foreach (var statement in interfaceDeclaration.Statements)
        {
            if (statement.Kind == TSSyntaxKind.PropertySignature)
            {
                var propertySignature = statement as PropertySignature;

                TypeSyntax type = propertySignature.QuestionToken == null ? ConvertType(propertySignature.Type) : SyntaxFactory.NullableType(ConvertType(propertySignature.Type));

                var propertyDeclaration = SyntaxFactory.PropertyDeclaration(type, propertySignature.Name.Text)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

                if (propertySignature.JSDoc != null)
                {
                    propertyDeclaration = AddComment(propertyDeclaration, propertySignature.JSDoc.Comment) as PropertyDeclarationSyntax;
                }

                @interface = @interface.AddMembers(propertyDeclaration);
            }
            else if (statement.Kind == TSSyntaxKind.MethodSignature)
            {
                var methodSignature = statement as MethodSignature;

                var methodDeclaration = SyntaxFactory.MethodDeclaration(ConvertType(methodSignature.Type), methodSignature.Name.Text)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                methodDeclaration = methodDeclaration.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                if (methodSignature.JSDoc != null)
                {
                    methodDeclaration = AddComment(methodDeclaration, methodSignature.JSDoc.Comment) as MethodDeclarationSyntax;
                }

                @interface = @interface.AddMembers(methodDeclaration);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        syntaxFactory = syntaxFactory.AddMembers(@interface);

        return syntaxFactory;
    }

    public static SyntaxNode AddComment(SyntaxNode node, string comment)
    {
        var comments = new List<SyntaxTrivia>();
        comments.Add(SyntaxFactory.Comment("/// <summary>"));
        comments.AddRange(comment.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(x => SyntaxFactory.Comment("/// " + x)));
        comments.Add(SyntaxFactory.Comment("/// </summary>"));

        return node.WithLeadingTrivia(new SyntaxTriviaList(comments));
    }

    private static TypeSyntax ConvertType(Node node)
    {
        switch (node.Kind)
        {
            case TSSyntaxKind.FirstLiteralToken:
                break;
            case TSSyntaxKind.StringLiteral:
                break;
            case TSSyntaxKind.QuestionToken:
                break;
            case TSSyntaxKind.Identifier:
                break;
            case TSSyntaxKind.ConstKeyword:
                break;
            case TSSyntaxKind.ExportKeyword:
                break;
            case TSSyntaxKind.NullKeyword:
                return SyntaxFactory.ParseTypeName("null");
            case TSSyntaxKind.VoidKeyword:
                return SyntaxFactory.ParseTypeName("void");
            case TSSyntaxKind.PrivateKeyword:
                break;
            case TSSyntaxKind.ProtectedKeyword:
                break;
            case TSSyntaxKind.StaticKeyword:
                break;
            case TSSyntaxKind.FirstContextualKeyword:
                break;
            case TSSyntaxKind.AnyKeyword:
                return SyntaxFactory.ParseTypeName("object");
            case TSSyntaxKind.BooleanKeyword:
                return SyntaxFactory.ParseTypeName("bool");
            case TSSyntaxKind.DeclareKeyword:
                break;
            case TSSyntaxKind.TypeOperator:
                break;
            case TSSyntaxKind.ReadonlyKeyword:
                break;
            case TSSyntaxKind.NumberKeyword:
                return SyntaxFactory.ParseTypeName("double");
            case TSSyntaxKind.StringKeyword:
                return SyntaxFactory.ParseTypeName("string");
            case TSSyntaxKind.UndefinedKeyword:
                break;
            case TSSyntaxKind.TypeParameter:
                break;
            case TSSyntaxKind.Parameter:
                break;
            case TSSyntaxKind.PropertySignature:
                break;
            case TSSyntaxKind.PropertyDeclaration:
                break;
            case TSSyntaxKind.MethodSignature:
                break;
            case TSSyntaxKind.MethodDeclaration:
                break;
            case TSSyntaxKind.Constructor:
                break;
            case TSSyntaxKind.IndexSignature:
                break;
            case TSSyntaxKind.TypeReference:
                // Handle Types
                var typeReference = (TypeReference)node;
                if (typeReference.TypeName.Text == "Array")
                {
                    return SyntaxFactory.ParseTypeName(ConvertType(typeReference.TypeArguments[0]) + "[]");
                }
                break;
            case TSSyntaxKind.FunctionType:
                break;
            case TSSyntaxKind.ConstructorType:
                break;
            case TSSyntaxKind.TypeLiteral:
                break;
            case TSSyntaxKind.ArrayType:
                return SyntaxFactory.ParseTypeName(ConvertType(((ArrayType)node).ElementType) + "[]");
            case TSSyntaxKind.TupleType:
                break;
            case TSSyntaxKind.UnionType:
                break;
            case TSSyntaxKind.IntersectionType:
                break;
            case TSSyntaxKind.IndexedAccessType:
                break;
            case TSSyntaxKind.MappedType:
                break;
            case TSSyntaxKind.ExpressionWithTypeArguments:
                break;
            case TSSyntaxKind.FirstStatement:
                break;
            case TSSyntaxKind.FunctionDeclaration:
                break;
            case TSSyntaxKind.ClassDeclaration:
                break;
            case TSSyntaxKind.InterfaceDeclaration:
                break;
            case TSSyntaxKind.TypeAliasDeclaration:
                break;
            case TSSyntaxKind.EnumDeclaration:
                break;
            case TSSyntaxKind.NamespaceExportDeclaration:
                break;
            case TSSyntaxKind.ImportDeclaration:
                break;
            case TSSyntaxKind.ImportClause:
                break;
            case TSSyntaxKind.NamespaceImport:
                break;
            case TSSyntaxKind.NamedImports:
                break;
            case TSSyntaxKind.ImportSpecifier:
                break;
            case TSSyntaxKind.ExportDeclaration:
                break;
            case TSSyntaxKind.NamedExports:
                break;
            case TSSyntaxKind.ExportSpecifier:
                break;
            case TSSyntaxKind.HeritageClause:
                break;
            case TSSyntaxKind.EnumMember:
                break;
            case TSSyntaxKind.SourceFile:
                break;
            case TSSyntaxKind.JSDocComment:
                break;
            default:
                break;
        }

        throw new NotImplementedException("");
    }
}
