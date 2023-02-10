namespace TestEffect.RecordStructs;

public partial class IgnoreEquality
{
    [Equatable]
    public partial record struct Sample(string Name, [property: IgnoreEquality] int Age);
}