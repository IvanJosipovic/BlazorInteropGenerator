# BlazorInteropGenerator

[![Nuget](https://img.shields.io/nuget/vpre/BlazorInteropGenerator.svg?style=flat-square)](https://www.nuget.org/packages/BlazorInteropGenerator)
[![Nuget)](https://img.shields.io/nuget/dt/BlazorInteropGenerator.svg?style=flat-square)](https://www.nuget.org/packages/BlazorInteropGenerator)
[![codecov](https://codecov.io/gh/IvanJosipovic/BlazorInteropGenerator/branch/alpha/graph/badge.svg?token=HmcWySWxe5)](https://codecov.io/gh/IvanJosipovic/BlazorInteropGenerator)

## What is this?

This project contains code and a Source Generator which can convert a TypeScript Definition to C# Interfaces.

## How to use the Source Generator

Create a C# Class Library Project.
Add the *.d.ts files that you want converted, also download all the dependencies.
File names should be the {PackageName}.d.ts without the Organization/.
Update the .csproj with the following settings.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BlazorInteropGenerator.SourceGenerator" Version="1.0.0-*" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
    <AdditionalFiles Include="*.d.ts" />
  </ItemGroup>

</Project>
```

Create a .cs file with this format, in this case, the TypeScript Definition must contain an interface called "InterfaceName"
The Source Generator will generate the interface and all its dependencies.

```c#

using BlazorInteropGenerator;

namespace BlazorInteropGenerator.Sample;

[BlazorInteropGenerator("TSD1.d.ts")]
public partial interface InterfaceName
{

}
```
