namespace TestEffect.RecordStructs;

public partial class GenericParameterEquality
{
    [Equatable]
    public partial record struct Sample<TName, TAge>(TName Name, TAge Age);
}