using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Xml;
using TSDParser.Class;
using TSDParser.Enums;

namespace BlazorInteropGenerator
{
    /// <summary>
    ///
    /// </summary>
    public static class BlazorInteropGenerator
    {
        static CompilationUnitSyntax GenerateObject(string typeScriptDefinition, string objectName, TSDParser.Enums.SyntaxKind syntaxKind)
        {
            var syntaxFactory = SyntaxFactory.CompilationUnit();

            var tsd = TSDParser.TSDParser.ParseDefinition(typeScriptDefinition);

            if (syntaxKind == TSDParser.Enums.SyntaxKind.InterfaceDeclaration)
            {
                var interfaceDeclaration = tsd.Statements.Where(x => x.Kind == syntaxKind).Cast<InterfaceDeclaration>().First(x => x.Name.Text == objectName);
                syntaxFactory = GenerateInterfaceDeclaration(syntaxFactory, interfaceDeclaration);
            }
            else if (syntaxKind == TSDParser.Enums.SyntaxKind.ClassDeclaration)
            {
                var classDeclaration = tsd.Statements.Where(x => x.Kind == syntaxKind).Cast<ClassDeclaration>().First(x => x.Name.Text == objectName);
            }
            else
            {
                throw new Exception("Not Supported");
            }

            return syntaxFactory;
        }

        public static CompilationUnitSyntax GenerateInterfaceDeclaration(CompilationUnitSyntax syntaxFactory, InterfaceDeclaration interfaceDeclaration)
        {
            var @interface = SyntaxFactory.InterfaceDeclaration(interfaceDeclaration.Name.Text);

            var @public = SyntaxFactory.Token(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword);

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
            @interface = @interface.AddModifiers(SyntaxFactory.Token(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword));

            syntaxFactory = syntaxFactory.AddMembers(@interface);

            return syntaxFactory;
        }
    }
}
