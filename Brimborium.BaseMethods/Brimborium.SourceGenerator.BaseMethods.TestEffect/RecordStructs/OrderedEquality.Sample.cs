namespace TestEffect.RecordStructs;

public partial class OrderedEquality
{
    [Equatable]
    public partial record struct Sample([property: OrderedEquality] string[] Addresses);
}