namespace TestEffect.Records;

public partial class NullableEquality
{
    [Equatable]
    public partial record Sample
    {
        [OrderedEquality] public string[]? Addresses { get; init; }
    }
}