namespace TestEffect.RecordStructs;

public partial class PrimitiveEquality
{
    [Equatable]
    public partial record struct Sample(string Name, int Age);
}
