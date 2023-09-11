using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TSDParser.Class;
using Xunit;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlazorInteropGenerator.Tests;

public class RealExampleTests
{
    [Fact]
    public async Task ApplicationInsightsTests()
    {
        var generator = new Generator();
        await generator.ParsePackage("applicationinsights-web", File.ReadAllText("definitions/applicationinsights-web.d.ts"));
        await generator.ParsePackage("applicationinsights-common", File.ReadAllText("definitions/applicationinsights-common.d.ts"));
        await generator.ParsePackage("applicationinsights-core-js", File.ReadAllText("definitions/applicationinsights-core-js.d.ts"));
        await generator.ParsePackage("applicationinsights-dependencies-js", File.ReadAllText("definitions/applicationinsights-dependencies-js.d.ts"));
        await generator.ParsePackage("applicationinsights-analytics-js", File.ReadAllText("definitions/applicationinsights-analytics-js.d.ts"));
        await generator.ParsePackage("ts-utils", File.ReadAllText("definitions/ts-utils.d.ts"));

        var syntaxFactory = generator.GenerateObjects("applicationinsights-web", "IApplicationInsights", TSDParser.Enums.SyntaxKind.InterfaceDeclaration, "Test");

        var code = syntaxFactory
           .NormalizeWhitespace()
           .ToFullString();
    }
}
