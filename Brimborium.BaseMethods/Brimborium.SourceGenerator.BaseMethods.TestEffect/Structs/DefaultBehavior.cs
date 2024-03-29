using TestEffect;

namespace TestEffect.Structs; 
public partial class DefaultBehavior
{
    public class EqualsTest : EqualityTestCase
    {
        public override bool Expected => true;
        public override object Factory1() => new Sample("Dave", 35);
        public override bool EqualsOperator(object value1, object value2) => ((Sample) value1).Equals((Sample) value2);
        public override bool NotEqualsOperator(object value1, object value2) => !((Sample) value1).Equals((Sample) value2);
    }
}