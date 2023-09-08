namespace BlazorInteropGenerator.Sample;

[BlazorInteropGenerator("TSD1.d.ts")]
public partial interface SomeType {}

public class SomeClass : SomeType
{
    public string? prop1 { get; set; }
    public string[] prop2 { get; set; }
    public double prop3 { get; set; }
    public object prop4 { get; set; }
    public bool prop5 { get; set; }
    public string prop6 { get; set; }

    public string? method()
    {
        throw new NotImplementedException();
    }

    public string method(string prop, double prop2)
    {
        throw new NotImplementedException();
    }
}