using System;

namespace BlazorInteropGenerator;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class BlazorInteropGeneratorAttribute : Attribute
{
    private string Name;

    /// <summary>
    /// Generates C# Interfaces from TypeScript Definitions
    /// </summary>
    /// <param name="name">NPM Package or path to TS Definition</param>
    public BlazorInteropGeneratorAttribute(string name)
    {
        Name = name;
    }
}