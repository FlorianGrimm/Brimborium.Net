namespace TestEffect.Classes;

public partial class IgnoreEquality
{
    [Equatable]
    public partial class Sample
    {
        public Sample(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public string Name { get; }

        [IgnoreEquality] public int Age { get; }
    }
}