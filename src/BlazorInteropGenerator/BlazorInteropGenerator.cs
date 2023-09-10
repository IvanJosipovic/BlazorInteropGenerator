using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSDParser.Class;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;
using TSSyntaxKind = TSDParser.Enums.SyntaxKind;

namespace BlazorInteropGenerator;

/// <summary>
/// Generates C# Code from TypeScript Definitions
/// </summary>
public static class Generator
{
    public static async Task<CompilationUnitSyntax> GenerateObjects(string typeScriptDefinition, string objectName, TSSyntaxKind syntaxKind, string @namespace)
    {
        var syntaxFactory = SyntaxFactory.CompilationUnit();

        var tsd = await TSDParser.TSDParser.ParseDefinition(typeScriptDefinition);

        var @namespaceObj = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(@namespace)).NormalizeWhitespace();

        if (syntaxKind == TSSyntaxKind.InterfaceDeclaration)
        {
            var interfaceDeclaration = tsd.Statements.Where(x => x.Kind == syntaxKind).Cast<InterfaceDeclaration>().First(x => x.Name.EscapedText == objectName);

            namespaceObj = namespaceObj.AddMembers(GenerateInterfaceDeclaration(syntaxFactory, interfaceDeclaration).Members.ToArray());
        }
        else if (syntaxKind == TSSyntaxKind.ClassDeclaration)
        {
            var classDeclaration = tsd.Statements.Where(x => x.Kind == syntaxKind).Cast<ClassDeclaration>().First(x => x.Name.EscapedText == objectName);

            namespaceObj = namespaceObj.AddMembers(GenerateClassDeclaration(syntaxFactory, classDeclaration).Members.ToArray());
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
        var @interface = SyntaxFactory.InterfaceDeclaration(interfaceDeclaration.Name.EscapedText);

        @interface = @interface.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        @interface = @interface.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

        if (interfaceDeclaration.JSDoc != null)
        {
            @interface = AddComment(@interface, interfaceDeclaration.JSDoc) as InterfaceDeclarationSyntax;
        }

        foreach (var statement in interfaceDeclaration.Members)
        {
            if (statement.Kind == TSSyntaxKind.PropertySignature)
            {
                var property = statement as PropertySignature;

                var type = property.QuestionToken == null ? ConvertType(property.Type) : SyntaxFactory.NullableType(ConvertType(property.Type));

                var propertyDeclaration = SyntaxFactory.PropertyDeclaration(type, property.Name.EscapedText)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

                if (property.JSDoc != null)
                {
                    propertyDeclaration = AddComment(propertyDeclaration, property.JSDoc) as PropertyDeclarationSyntax;
                }

                @interface = @interface.AddMembers(propertyDeclaration);
            }
            else if (statement.Kind == TSSyntaxKind.MethodSignature)
            {
                var method = statement as MethodSignature;

                var returnType = method.QuestionToken == null ? ConvertType(method.Type) : SyntaxFactory.NullableType(ConvertType(method.Type));

                var methodDeclaration = SyntaxFactory.MethodDeclaration(returnType, method.Name.EscapedText)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                methodDeclaration = methodDeclaration.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                if (method.JSDoc != null)
                {
                    methodDeclaration = AddComment(methodDeclaration, method.JSDoc) as MethodDeclarationSyntax;
                }

                if (method.Parameters?.Any() == true)
                {
                    var parameters = new List<ParameterSyntax>();

                    foreach (var item in method.Parameters)
                    {
                        var type = item.QuestionToken == null ? ConvertType(item.Type) : SyntaxFactory.NullableType(ConvertType(item.Type));

                        var param = SyntaxFactory.Parameter(SyntaxFactory.Identifier(item.Name.EscapedText)).WithType(type);
                        parameters.Add(param);
                    }

                    methodDeclaration = methodDeclaration.WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)));
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

    public static CompilationUnitSyntax GenerateClassDeclaration(CompilationUnitSyntax syntaxFactory, ClassDeclaration classDeclaration)
    {
        var @class = SyntaxFactory.ClassDeclaration(classDeclaration.Name.EscapedText);

        @class = @class.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        @class = @class.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

        if (classDeclaration.JSDoc != null)
        {
            @class = AddComment(@class, classDeclaration.JSDoc) as ClassDeclarationSyntax;
        }

        foreach (var statement in classDeclaration.Members)
        {
            if (statement.Kind == TSSyntaxKind.PropertyDeclaration)
            {
                var property = statement as PropertyDeclaration;

                var type = property.QuestionToken == null ? ConvertType(property.Type) : SyntaxFactory.NullableType(ConvertType(property.Type));

                var propertyDeclaration = SyntaxFactory.PropertyDeclaration(type, property.Name.EscapedText)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

                if (property.JSDoc != null)
                {
                    propertyDeclaration = AddComment(propertyDeclaration, property.JSDoc) as PropertyDeclarationSyntax;
                }

                @class = @class.AddMembers(propertyDeclaration);
            }
            else if (statement.Kind == TSSyntaxKind.MethodDeclaration)
            {
                var method = statement as MethodDeclaration;

                var returnType = method.QuestionToken == null ? ConvertType(method.Type) : SyntaxFactory.NullableType(ConvertType(method.Type));

                var methodDeclaration = SyntaxFactory.MethodDeclaration(returnType, method.Name.EscapedText)
                    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                methodDeclaration = methodDeclaration.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                if (method.JSDoc != null)
                {
                    methodDeclaration = AddComment(methodDeclaration, method.JSDoc) as MethodDeclarationSyntax;
                }

                if (method.Parameters?.Any() == true)
                {
                    var parameters = new List<ParameterSyntax>();

                    foreach (var item in method.Parameters)
                    {
                        var type = item.QuestionToken == null ? ConvertType(item.Type) : SyntaxFactory.NullableType(ConvertType(item.Type));

                        var param = SyntaxFactory.Parameter(SyntaxFactory.Identifier(item.Name.EscapedText)).WithType(type);
                        parameters.Add(param);
                    }

                    methodDeclaration = methodDeclaration.WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)));
                }

                methodDeclaration = methodDeclaration.WithBody(SyntaxFactory.Block(SyntaxFactory.ParseStatement("throw new NotImplementedException();")));

                @class = @class.AddMembers(methodDeclaration);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        syntaxFactory = syntaxFactory.AddMembers(@class);

        return syntaxFactory;
    }

    public static SyntaxNode AddComment(SyntaxNode node, List<JSDoc> jsDocs)
    {
        var comments = new List<SyntaxTrivia>();
        comments.Add(SyntaxFactory.Comment("/// <summary>"));
        comments.AddRange(jsDocs.Where(x => x.Comment is not null)
                        .SelectMany(x => x.Comment.Where(y => y is JSDocText))
                        .SelectMany(x => ((JSDocText)x).Text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => SyntaxFactory.Comment("/// " + x))));
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
                var typeReference = (TSDParser.Class.TypeReference)node;
                if (typeReference.TypeName.EscapedText == "Array")
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
            case TSSyntaxKind.JSDoc:
                break;
            default:
                break;
        }

        throw new NotImplementedException("");
    }
}
