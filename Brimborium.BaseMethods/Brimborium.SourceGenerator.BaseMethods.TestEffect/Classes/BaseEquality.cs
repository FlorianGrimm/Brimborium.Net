namespace TestEffect.Classes;
public partial class BaseEquality {
    public class EqualsTest : EqualityTestCase<Person> {
        private void x() {
            var person1 = new TestEffect.Classes.BaseEquality.Person(1);
            person1.Equals(person1);
            var x = person1.GetHashCode();
        }
        public override object Factory1() => new Manager(25, "IT");
        public override bool EqualsOperator(object value1, object value2) => (Manager)value1 == (Manager)value2;
        public override bool NotEqualsOperator(object value1, object value2) => (Manager)value1 != (Manager)value2;
    }

    public class NotEqualsTest : EqualityTestCase<Person> {
        public override bool Expected => false;
        public override object Factory1() => new Manager(25, "IT");
        public override object Factory2() => new Manager(25, "Sales");
        public override bool EqualsOperator(object value1, object value2) => (Manager)value1 == (Person)value2;
        public override bool NotEqualsOperator(object value1, object value2) => (Manager)value1 != (Person)value2;
    }
}