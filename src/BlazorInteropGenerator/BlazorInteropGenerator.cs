using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSDParser.Class;
using TSDParser.Class.Keywords;
using TSDParser.Enums;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;
using TSSyntaxKind = TSDParser.Enums.SyntaxKind;

namespace BlazorInteropGenerator;

/// <summary>
/// Generates C# Code from TypeScript Definitions
/// </summary>
public class Generator
{
    Dictionary<string, SourceFile> Packages = new();

    CompilationUnitSyntax compilationUnit = SyntaxFactory.CompilationUnit();

    NamespaceDeclarationSyntax namespaceDeclaration;

    Queue<(string, string)> missingObjects = new();

    /// <summary>
    /// List of C# keywords
    /// </summary>
    private static readonly List<string> keywords = new List<string>()
    {
        "abstract",
        "as",
        "base",
        "bool",
        "break",
        "byte",
        "case",
        "catch",
        "char",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "double",
        "else",
        "enum",
        "event",
        "explicit",
        "extern",
        "false",
        "finally",
        "fixed",
        "float",
        "for",
        "foreach",
        "goto",
        "if",
        "implicit",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "lock",
        "long",
        "namespace",
        "new",
        "null",
        "object",
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "short",
        "sizeof",
        "stackalloc",
        "static",
        "string",
        "struct",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "virtual",
        "void",
        "volatile",
        "while"
    };

    public async Task ParsePackage(string packageName, string packageContent)
    {
        var tsd = await TSDParser.TSDParser.ParseDefinition(packageContent);

        Packages.Add(packageName, tsd);
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
                var baseTypes = new List<BaseTypeSyntax>();

                foreach (var type in item.Types)
                {
                    baseTypes.Add(SyntaxFactory.SimpleBaseType(ConvertType(typeScriptDefinitionName, type)));
                }

                @interface = @interface.WithBaseList(SyntaxFactory.BaseList(SyntaxFactory.SeparatedList(baseTypes)));
            }
        }

        foreach (var statement in interfaceDeclaration.Members)
        {
            if (statement.Kind == TSSyntaxKind.PropertySignature)
            {
                var property = statement as PropertySignature;

                if (property.Type is FunctionType)
                {
                    var functionType = (FunctionType) property.Type;

                    var type =  ConvertType(typeScriptDefinitionName, functionType.Type, property.QuestionToken);

                    var methodDeclaration = GenerateMethod(typeScriptDefinitionName, property.Name, functionType.Parameters, type, property.JSDoc);

                    @interface = @interface.AddMembers(methodDeclaration);
                }
                else
                {
                    var type =  ConvertType(typeScriptDefinitionName, property.Type, property.QuestionToken);

                    var propertyDeclaration = SyntaxFactory.PropertyDeclaration(type, CapitalizeFistChar(property.Name.EscapedText))
                        .AddAccessorListAccessors(
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

                    if (property.JSDoc != null)
                    {
                        propertyDeclaration = AddComment(propertyDeclaration, property.JSDoc) as PropertyDeclarationSyntax;
                    }

                    @interface = @interface.AddMembers(propertyDeclaration);
                }
            }
            else if (statement.Kind == TSSyntaxKind.MethodSignature)
            {
                var method = statement as MethodSignature;

                var returnType = ConvertType(typeScriptDefinitionName, method.Type, method.QuestionToken);

                var methodDeclaration = GenerateMethod(typeScriptDefinitionName, method.Name, method.Parameters, returnType, method.JSDoc);

                @interface = @interface.AddMembers(methodDeclaration);
            }
            else
            {
                //throw new NotSupportedException();
            }
        }

        return @interface;
    }

    private MethodDeclarationSyntax GenerateMethod(string typeScriptDefinitionName, Identifier identifier, List<Parameter>? Parameters, TypeSyntax returnType, List<JSDoc>? JSDoc)
    {
        var methodDeclaration = SyntaxFactory.MethodDeclaration(returnType, CapitalizeFistChar(identifier.EscapedText));

        methodDeclaration = methodDeclaration.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

        if (JSDoc != null)
        {
            methodDeclaration = AddComment(methodDeclaration, JSDoc) as MethodDeclarationSyntax;
        }

        if (Parameters?.Any() == true)
        {
            var parameters = new List<ParameterSyntax>();

            foreach (var item in Parameters)
            {
                var type = ConvertType(typeScriptDefinitionName, item.Type, item.QuestionToken);

                var param = SyntaxFactory.Parameter(SyntaxFactory.Identifier(GetCleanParameterName(item.Name.EscapedText))).WithType(type);
                parameters.Add(param);
            }

            methodDeclaration = methodDeclaration.WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)));

        }

        return methodDeclaration;
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

                var type = ConvertType(typeScriptDefinitionName, property.Type, property.QuestionToken);

                var propertyDeclaration = SyntaxFactory.PropertyDeclaration(type, CapitalizeFistChar(property.Name.EscapedText))
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

                var returnType = ConvertType(typeScriptDefinitionName, method.Type, method.QuestionToken);

                var methodDeclaration = GenerateMethod(typeScriptDefinitionName, method.Name, method.Parameters, returnType, method.JSDoc);

                methodDeclaration = methodDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

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

    private TypeSyntax ConvertType(string typeScriptDefinitionName, Node node, QuestionToken? questionToken = null)
    {
        TypeSyntax type = SyntaxFactory.ParseTypeName("object");

        switch (node.Kind)
        {
            case TSSyntaxKind.NullKeyword:
                type = SyntaxFactory.ParseTypeName("null");
                break;
            case TSSyntaxKind.VoidKeyword:
                // Void can not be nullable
                return SyntaxFactory.ParseTypeName("void");
            case TSSyntaxKind.AnyKeyword:
                type = SyntaxFactory.ParseTypeName("object");
                break;
            case TSSyntaxKind.BooleanKeyword:
                type = SyntaxFactory.ParseTypeName("bool");
                break;
            case TSSyntaxKind.NumberKeyword:
                type = SyntaxFactory.ParseTypeName("double");
                break;
            case TSSyntaxKind.StringKeyword:
                type = SyntaxFactory.ParseTypeName("string");
                break;
            case TSSyntaxKind.TypeReference:
                // Handle Types
                var typeReference = (TypeReference)node;
                if (typeReference.TypeName.EscapedText == "Array")
                {
                    type = SyntaxFactory.ParseTypeName(ConvertType(typeScriptDefinitionName, typeReference.TypeArguments[0]) + "[]");
                }
                else if (typeReference.TypeName.EscapedText == "Date")
                {
                    type = SyntaxFactory.ParseTypeName("DateTime");
                }
                else
                {
                    // Not a known type
                    if (!missingObjects.Contains((typeScriptDefinitionName, typeReference.TypeName.EscapedText)))
                    {
                        missingObjects.Enqueue((typeScriptDefinitionName, typeReference.TypeName.EscapedText));
                    }

                    type = SyntaxFactory.ParseTypeName(typeReference.TypeName.EscapedText);
                }
                break;
            case TSSyntaxKind.FunctionType:
                var functionType = (FunctionType)node;

                if (functionType.Type is VoidKeyword)
                {
                    // Action

                    if (functionType.Parameters.Count == 0)
                    {
                        type = SyntaxFactory.ParseTypeName("System.Action");
                    }
                    else
                    {
                        var types = new List<TypeSyntax>();

                        if (functionType.Parameters.Count > 0)
                        {
                            types.AddRange(functionType.Parameters.Select(x => ConvertType(typeScriptDefinitionName, x.Type)));
                        }

                        type = SyntaxFactory.GenericName(SyntaxFactory.Identifier("System.Action"), SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(types)));
                    }

                }
                else
                {
                    // Func
                    var types = new List<TypeSyntax>();

                    if (functionType.Parameters.Count > 0)
                    {
                        types.AddRange(functionType.Parameters.Select(x => ConvertType(typeScriptDefinitionName, x.Type)));
                    }

                    types.Add(ConvertType(typeScriptDefinitionName, functionType.Type));

                    type = SyntaxFactory.GenericName(SyntaxFactory.Identifier("System.Func"), SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(types)));
                }

                break;
            case TSSyntaxKind.ConstructorType:
                break;
            case TSSyntaxKind.TypeLiteral:
                var typeLiteral = (TypeLiteral)node;

                if (typeLiteral.Members.Count == 1 && typeLiteral.Members[0] is IndexSignature)
                {
                    var indexSignature = typeLiteral.Members[0] as IndexSignature;

                    // Dictionary
                    type = SyntaxFactory.GenericName(SyntaxFactory.Identifier("System.Collections.Generic.Dictionary"), SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(new List<TypeSyntax>()
                    {
                        ConvertType(typeScriptDefinitionName, indexSignature.Parameters[0].Type),
                        ConvertType(typeScriptDefinitionName, indexSignature.Type)
                    })));
                }
                break;
            case TSSyntaxKind.ArrayType:
                type = SyntaxFactory.ParseTypeName(ConvertType(typeScriptDefinitionName, ((ArrayType)node).ElementType) + "[]");
                break;
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

                type = SyntaxFactory.ParseTypeName(expressionWithTypeArguments.Expression.EscapedText);
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
        }

        return questionToken == null ? type : SyntaxFactory.NullableType(type);
    }

    private string CapitalizeFistChar(string test)
    {
        return char.ToUpperInvariant(test[0]) + test.Substring(1);
    }

    private string GetCleanParameterName(string propName)
    {
        if (keywords.Contains(propName))
        {
            return "@" + propName;
        }

        return propName;
    }
}
