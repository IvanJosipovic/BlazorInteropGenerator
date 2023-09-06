using System;

namespace BlazorInteropGenerator
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class BlazorInteropGeneratorAttribute : Attribute
    {
        private string Name;

        private string ObjectName;

        /// <summary>
        /// Generates C# Interfaces from TypeScript Definitions
        /// </summary>
        /// <param name="name">NPM Package or path to TS Definition</param>
        public BlazorInteropGeneratorAttribute(string name, string objectName)
        {
            Name = name;
            ObjectName = objectName;
        }
    }
}
