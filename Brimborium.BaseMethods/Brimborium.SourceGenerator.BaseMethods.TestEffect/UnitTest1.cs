namespace TestEffect; 
public class UnitTest1 {
    [Fact]
    public void Test1() {
        var person1 = new TestEffect.Classes.BaseEquality.Person(1);
        person1.Equals(person1);
        var x=person1.GetHashCode();
    }
}