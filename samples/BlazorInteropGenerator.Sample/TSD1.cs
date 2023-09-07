using BlazorInteropGenerator;

namespace BlazorInteropGenerator.Sample;

[BlazorInteropGenerator("TSD1.d.ts")]
public partial interface SomeType {}

/// <inheritdoc>/>
public class SomeClass : SomeType
{
    public string test1 { get; set; }
    public string[] test2 { get; set; }
    public double test3 { get; set; }
    public object test4 { get; set; }
    public bool test5 { get; set; }
    public string test6 { get; set; }
}