namespace TestEffect.Records;

public partial class DefaultBehavior
{
    public partial class RecordsWithArray
    {
        public record Sample(string Name, int Age, string[] Addresses);
    }
}