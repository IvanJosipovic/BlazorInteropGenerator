# BlazorInteropGenerator

[![Nuget](https://img.shields.io/nuget/vpre/BlazorInteropGenerator.svg?style=flat-square)](https://www.nuget.org/packages/BlazorInteropGenerator)
[![Nuget)](https://img.shields.io/nuget/dt/BlazorInteropGenerator.svg?style=flat-square)](https://www.nuget.org/packages/BlazorInteropGenerator)
[![codecov](https://codecov.io/gh/IvanJosipovic/BlazorInteropGenerator/branch/alpha/graph/badge.svg?token=HmcWySWxe5)](https://codecov.io/gh/IvanJosipovic/BlazorInteropGenerator)

## What is this?

This project contains code and a Source Generator which can convert a TypeScript Defenition to a C# Interface.

## How to use the Source Generator
Create a C# Class Library Project.
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

Create a Class file with this format.
```c#
using BlazorInteropGenerator;

namespace BlazorInteropGenerator.Sample;

[BlazorInteropGenerator("TSD1.d.ts", "InterfaceName")]
public partial interface InterfaceName
{

}

```