namespace TestEffect.Records;

public partial class IgnoreEquality
{
    [Equatable]
    public partial record Sample(string Name, [property: IgnoreEquality] int Age);
}