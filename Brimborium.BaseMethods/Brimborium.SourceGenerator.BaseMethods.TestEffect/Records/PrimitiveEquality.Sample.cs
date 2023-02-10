namespace TestEffect.Records;

public partial class PrimitiveEquality
{
    [Equatable]
    public partial record Sample(string Name, int Age);
}