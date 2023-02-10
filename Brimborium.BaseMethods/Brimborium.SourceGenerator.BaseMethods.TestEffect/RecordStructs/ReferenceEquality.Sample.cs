namespace TestEffect.RecordStructs;

public partial class ReferenceEquality
{
    [Equatable]
    public partial record struct Sample([property: ReferenceEquality] string Name);
}