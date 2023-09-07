using BlazorInteropGenerator;

namespace BlazorInteropGenerator.Sample;

[BlazorInteropGenerator("TSD1.d.ts", "SomeType")]
public partial interface SomeType
{

}

public class SomeClass : SomeType
{
    /// <inheritdoc/>
    public string test { get; set; }
    public string test3 { get; set; }
}