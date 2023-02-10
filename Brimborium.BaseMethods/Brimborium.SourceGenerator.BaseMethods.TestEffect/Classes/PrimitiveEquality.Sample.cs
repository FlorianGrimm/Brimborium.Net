namespace TestEffect.Classes;

public partial class PrimitiveEquality
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
        public int Age { get; }
    }
}