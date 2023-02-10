namespace TestEffect.RecordStructs;

public partial class ExplicitMode
{
    [Equatable(Explicit = true)]
    public partial record struct Sample(string Name, [property: DefaultEquality]int Age);
}