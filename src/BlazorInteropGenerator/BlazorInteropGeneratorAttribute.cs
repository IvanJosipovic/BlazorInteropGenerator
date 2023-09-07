using System;
using TSDParser.Enums;

namespace BlazorInteropGenerator;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class BlazorInteropGeneratorAttribute : Attribute
{
    private string Name;

    private string ObjectName;

    /// <summary>
    /// Generates C# Interfaces from TypeScript Definitions
    /// </summary>
    /// <param name="name">NPM Package or path to TS Definition</param>
    /// <param name="objectName">Object to Generate</param>
    public BlazorInteropGeneratorAttribute(string name, string? objectName)
    {
        Name = name;
        ObjectName = objectName;
    }
}