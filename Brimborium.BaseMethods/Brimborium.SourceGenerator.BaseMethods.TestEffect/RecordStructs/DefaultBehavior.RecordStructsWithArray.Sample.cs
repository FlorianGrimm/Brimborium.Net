namespace TestEffect.RecordStructs;

public partial class DefaultBehavior
{
    public partial class RecordStructsWithArray
    {
        public record struct Sample(string Name, int Age, string[] Addresses);
    }
}