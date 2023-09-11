using Microsoft.AspNetCore.Http.Features;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSDParser.Class;
using TSDParser.Enums;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;
using TSSyntaxKind = TSDParser.Enums.SyntaxKind;

namespace BlazorInteropGenerator;

/// <summary>
/// Generates C# Code from TypeScript Definitions
/// </summary>
public class Generator
{
    private Dictionary<string, SourceFile> Packages = new();

    CompilationUnitSyntax compilationUnit = SyntaxFactory.CompilationUnit();

    NamespaceDeclarationSyntax namespaceDeclaration;

    private Queue<(string, string)> missingObjects = new();

    public async Task ParsePackage(string packageName, string packageContent)
    {
        var tsd = await TSDParser.TSDParser.ParseDefinition(packageContent);

        Packages.Add(packageName, tsd);
    }

    private (string, InterfaceDeclaration)? GetInterface(string typeScriptDefinitionName, string objectName)
    {
        if (!Packages.ContainsKey(typeScriptDefinitionName))
        {
            return null;
        }

        var localInterface = Packages[typeScriptDefinitionName].Statements.Where(x => x.Kind == TSSyntaxKind.InterfaceDeclaration).Cast<InterfaceDeclaration>().FirstOrDefault(x => x.Name.EscapedText == objectName);

        if (localInterface != null)
        {
            return (typeScriptDefinitionName, localInterface);
        }

        var ext = Packages[typeScriptDefinitionName].Statements.Where(x => x.Kind == TSSyntaxKind.ImportDeclaration).Cast<ImportDeclaration>().FirstOrDefault(x => ((NamedImports)x.ImportClause.NamedBindings).Elements.First().Name.EscapedText == objectName);

        if (ext == null)
        {
            return null;
        }

        var packageName = ext.ModuleSpecifier.Text.Contains("/") ? ext.ModuleSpecifier.Text.Substring(ext.ModuleSpecifier.Text.LastIndexOf("/") + 1) : ext.ModuleSpecifier.Text;

        var externalInterface = GetInterface(packageName, ((NamedImports)ext.ImportClause.NamedBindings).Elements.First().Name.EscapedText);

        return externalInterface;
    }

    private (string, ClassDeclaration)? GetClass(string typeScriptDefinitionName, string objectName)
    {
        if (!Packages.ContainsKey(typeScriptDefinitionName))
        {
            return null;
        }

        var localClass = Packages[typeScriptDefinitionName].Statements.Where(x => x.Kind == TSSyntaxKind.ClassDeclaration).Cast<ClassDeclaration>().FirstOrDefault(x => x.Name.EscapedText == objectName);

        if (localClass != null)
        {
            return (typeScriptDefinitionName, localClass);
        }

        var ext = Packages[typeScriptDefinitionName].Statements.Where(x => x.Kind == TSSyntaxKind.ImportDeclaration).Cast<ImportDeclaration>().FirstOrDefault(x => ((NamedImports)x.ImportClause.NamedBindings).Elements.First().Name.EscapedText == objectName);

        if (ext == null)
        {
            return null;
        }

        var packageName = ext.ModuleSpecifier.Text.Contains("/") ? ext.ModuleSpecifier.Text.Substring(ext.ModuleSpecifier.Text.LastIndexOf("/") + 1) : ext.ModuleSpecifier.Text;

        var externalClass = GetClass(packageName, ((NamedImports)ext.ImportClause.NamedBindings).Elements.First().Name.EscapedText);

        return externalClass;
    }

    public CompilationUnitSyntax GenerateObjects(string typeScriptDefinitionName, string objectName, TSSyntaxKind syntaxKind, string @namespace)
    {
        namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(@namespace)).NormalizeWhitespace();

        if (syntaxKind == TSSyntaxKind.InterfaceDeclaration)
        {
            var interfaceDeclaration = GetInterface(typeScriptDefinitionName, objectName);

            namespaceDeclaration = namespaceDeclaration.AddMembers(GenerateInterfaceDeclaration(interfaceDeclaration.Value.Item1, interfaceDeclaration.Value.Item2));
        }
        else if (syntaxKind == TSSyntaxKind.ClassDeclaration)
        {
            var classDeclaration = GetClass(typeScriptDefinitionName, objectName);

            namespaceDeclaration = namespaceDeclaration.AddMembers(GenerateClassDeclaration(classDeclaration.Value.Item1, classDeclaration.Value.Item2));
        }
        else
        {
            throw new NotSupportedException("Kind not supported " + syntaxKind.ToString());
        }

        // Generate missing Interfaces
        while (missingObjects.Count > 0)
        {
            var item = missingObjects.Dequeue();

            if (namespaceDeclaration.Members.Any(x => (x.IsKind(SyntaxKind.InterfaceDeclaration) && ((InterfaceDeclarationSyntax)x).Identifier.Text == item.Item2) || (x.IsKind(SyntaxKind.ClassDeclaration) && ((ClassDeclarationSyntax)x).Identifier.Text == item.Item2)))
            {
                continue;
            }

            var @newInterface = GetInterface(item.Item1, item.Item2);

            if (newInterface != null)
            {
                namespaceDeclaration = namespaceDeclaration.AddMembers(GenerateInterfaceDeclaration(@newInterface.Value.Item1, @newInterface.Value.Item2));
            }
            else
            {
                var @newClass = GetClass(item.Item1, item.Item2);

                if (@newClass != null)
                {
                    namespaceDeclaration = namespaceDeclaration.AddMembers(GenerateClassDeclaration(@newClass.Value.Item1, @newClass.Value.Item2));
                }
            }
        }

        compilationUnit = compilationUnit.AddMembers(namespaceDeclaration);

        return compilationUnit;
    }

    private MemberDeclarationSyntax GenerateInterfaceDeclaration(string typeScriptDefinitionName, InterfaceDeclaration interfaceDeclaration)
    {
        var @interface = SyntaxFactory.InterfaceDeclaration(interfaceDeclaration.Name.EscapedText);

        @interface = @interface.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
        @interface = @interface.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

        if (interfaceDeclaration.JSDoc != null)
        {
            @interface = AddComment(@interface, interfaceDeclaration.JSDoc) as InterfaceDeclarationSyntax;
        }

        if (interfaceDeclaration.HeritageClauses != null)
        {
            foreach (var item in interfaceDeclaration.HeritageClauses)
            {
                foreach (var type in item.Types)
                {
                    @interface.AddMembers(SyntaxFactory.IncompleteMember(ConvertType(typeScriptDefinitionName, type)));
                }
            }
        }

        foreach (var statement in interfaceDeclaration.Members)
        {
            if (statement.Kind == TSSyntaxKind.PropertySignature)
            {
                var property = statement as PropertySignature;

                var type = property.QuestionToken == null ? ConvertType(typeScriptDefinitionName, property.Type) : SyntaxFactory.NullableType(ConvertType(typeScriptDefinitionName, property.Type));

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

                var returnType = method.QuestionToken == null ? ConvertType(typeScriptDefinitionName, method.Type) : SyntaxFactory.NullableType(ConvertType(typeScriptDefinitionName, method.Type));

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
                        var type = item.QuestionToken == null ? ConvertType(typeScriptDefinitionName, item.Type) : SyntaxFactory.NullableType(ConvertType(typeScriptDefinitionName, item.Type));

                        var param = SyntaxFactory.Parameter(SyntaxFactory.Identifier(item.Name.EscapedText)).WithType(type);
                        parameters.Add(param);
                    }

                    methodDeclaration = methodDeclaration.WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)));
                }

                @interface = @interface.AddMembers(methodDeclaration);
            }
            else
            {
                //throw new NotSupportedException();
            }
        }

        return @interface;
    }

    private MemberDeclarationSyntax GenerateClassDeclaration(string typeScriptDefinitionName, ClassDeclaration classDeclaration)
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

                var type = property.QuestionToken == null ? ConvertType(typeScriptDefinitionName, property.Type) : SyntaxFactory.NullableType(ConvertType(typeScriptDefinitionName, property.Type));

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

                var returnType = method.QuestionToken == null ? ConvertType(typeScriptDefinitionName, method.Type) : SyntaxFactory.NullableType(ConvertType(typeScriptDefinitionName, method.Type));

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
                        var type = item.QuestionToken == null ? ConvertType(typeScriptDefinitionName, item.Type) : SyntaxFactory.NullableType(ConvertType(typeScriptDefinitionName, item.Type));

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
                //throw new NotSupportedException("Kind not Supported " + statement.Kind);
            }
        }

        return @class;
    }

    private SyntaxNode AddComment(SyntaxNode node, List<JSDoc> jsDocs)
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

    private TypeSyntax ConvertType(string typeScriptDefinitionName, Node node)
    {
        switch (node.Kind)
        {
            case TSSyntaxKind.NullKeyword:
                return SyntaxFactory.ParseTypeName("null");
            case TSSyntaxKind.VoidKeyword:
                return SyntaxFactory.ParseTypeName("void");
            case TSSyntaxKind.AnyKeyword:
                return SyntaxFactory.ParseTypeName("object");
            case TSSyntaxKind.BooleanKeyword:
                return SyntaxFactory.ParseTypeName("bool");
            case TSSyntaxKind.NumberKeyword:
                return SyntaxFactory.ParseTypeName("double");
            case TSSyntaxKind.StringKeyword:
                return SyntaxFactory.ParseTypeName("string");
            case TSSyntaxKind.TypeReference:
                // Handle Types
                var typeReference = (TypeReference)node;
                if (typeReference.TypeName.EscapedText == "Array")
                {
                    return SyntaxFactory.ParseTypeName(ConvertType(typeScriptDefinitionName, typeReference.TypeArguments[0]) + "[]");
                }
                else if (typeReference.TypeName.EscapedText == "Date")
                {
                    return SyntaxFactory.ParseTypeName("DateTime");
                }
                else
                {
                    // Not a known type
                    if (!missingObjects.Contains((typeScriptDefinitionName, typeReference.TypeName.EscapedText)))
                    {
                        missingObjects.Enqueue((typeScriptDefinitionName, typeReference.TypeName.EscapedText));
                    }

                    return SyntaxFactory.ParseTypeName(typeReference.TypeName.EscapedText);
                }
            case TSSyntaxKind.FunctionType:
                break;
            case TSSyntaxKind.ConstructorType:
                break;
            case TSSyntaxKind.TypeLiteral:
                var typeLiteral = (TypeLiteral)node;

                if (typeLiteral.Members.Count == 1 && typeLiteral.Members[0] is IndexSignature)
                {
                    var indexSignature = typeLiteral.Members[0] as IndexSignature;

                    // Dictionary
                    return SyntaxFactory.GenericName(SyntaxFactory.Identifier("System.Collections.Generic.Dictionary"), SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(new List<TypeSyntax>()
                    {
                        ConvertType(typeScriptDefinitionName, indexSignature.Parameters[0].Type),
                        ConvertType(typeScriptDefinitionName, indexSignature.Type)
                    })));
                }

                break;
            case TSSyntaxKind.ArrayType:
                return SyntaxFactory.ParseTypeName(ConvertType(typeScriptDefinitionName, ((ArrayType)node).ElementType) + "[]");
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
                var expressionWithTypeArguments = (ExpressionWithTypeArguments)node;

                // Not a known type
                if (!missingObjects.Contains((typeScriptDefinitionName, expressionWithTypeArguments.Expression.EscapedText)))
                {
                    missingObjects.Enqueue((typeScriptDefinitionName, expressionWithTypeArguments.Expression.EscapedText));
                }

                return SyntaxFactory.ParseTypeName(expressionWithTypeArguments.Expression.EscapedText);
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
            default:
                break;
        }

        return SyntaxFactory.ParseTypeName("object");
    }
}
