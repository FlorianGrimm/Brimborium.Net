namespace TestEffect.Records;

public partial class ExplicitMode
{
    [Equatable(Explicit = true)]
    public partial record Sample(string Name, [property: DefaultEquality]int Age);
}