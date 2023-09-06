using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Xml;
using TSDParser.Class;
using TSDParser.Enums;
using TSDParser.Interfaces;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;
using TSSyntaxKind = TSDParser.Enums.SyntaxKind;


namespace BlazorInteropGenerator
{
    /// <summary>
    ///
    /// </summary>
    public static class BlazorInteropGenerator
    {
        static CompilationUnitSyntax GenerateObject(string typeScriptDefinition, string objectName, TSSyntaxKind syntaxKind)
        {
            var syntaxFactory = SyntaxFactory.CompilationUnit();

            var tsd = TSDParser.TSDParser.ParseDefinition(typeScriptDefinition);

            if (syntaxKind == TSSyntaxKind.InterfaceDeclaration)
            {
                var interfaceDeclaration = tsd.Statements.Where(x => x.Kind == syntaxKind).Cast<InterfaceDeclaration>().First(x => x.Name.Text == objectName);
                syntaxFactory = GenerateInterfaceDeclaration(syntaxFactory, interfaceDeclaration);
            }
            else if (syntaxKind == TSSyntaxKind.ClassDeclaration)
            {
                var classDeclaration = tsd.Statements.Where(x => x.Kind == syntaxKind).Cast<ClassDeclaration>().First(x => x.Name.Text == objectName);
            }
            else
            {
                throw new NotSupportedException();
            }

            return syntaxFactory;
        }

        public static CompilationUnitSyntax GenerateInterfaceDeclaration(CompilationUnitSyntax syntaxFactory, InterfaceDeclaration interfaceDeclaration)
        {
            var @interface = SyntaxFactory.InterfaceDeclaration(interfaceDeclaration.Name.Text);

            var @public = SyntaxFactory.Token(SyntaxKind.PublicKeyword);

            if (interfaceDeclaration.JSDoc != null)
            {
                var comment = SyntaxFactory.Comment("/// " + interfaceDeclaration.JSDoc.Comment);

                //var comment2 = SyntaxFactory.SyntaxTrivia(Microsoft.CodeAnalysis.CSharp.SyntaxKind.DocumentationCommentExteriorTrivia, interfaceDeclaration.JSDoc.Comment);

                @public = @public.WithLeadingTrivia(new SyntaxTriviaList(new SyntaxTrivia[]
                {
                    comment
                }));
            }

            @interface = @interface.AddModifiers(@public);
            @interface = @interface.AddModifiers(SyntaxFactory.Token(SyntaxKind.PartialKeyword));

            foreach (var statement in interfaceDeclaration.Statements)
            {
                if (statement.Kind == TSSyntaxKind.PropertySignature)
                {
                    var propertySignature = statement as PropertySignature;

                    var propertyDeclaration = SyntaxFactory.PropertyDeclaration(ConvertType(propertySignature.Type), propertySignature.Name.Text)
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                        .AddAccessorListAccessors(
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

                    @interface = @interface.AddMembers(propertyDeclaration);
                }
                else if (statement.Kind == TSSyntaxKind.MethodSignature)
                {
                    var methodSignature = statement as MethodSignature;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            syntaxFactory = syntaxFactory.AddMembers(@interface);

            return syntaxFactory;
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
                    break;
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
                    break;
                case TSSyntaxKind.BooleanKeyword:
                    break;
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
                    break;
                case TSSyntaxKind.FunctionType:
                    break;
                case TSSyntaxKind.ConstructorType:
                    break;
                case TSSyntaxKind.TypeLiteral:
                    break;
                case TSSyntaxKind.ArrayType:
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
}
