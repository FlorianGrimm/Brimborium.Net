using TestEffect;

namespace TestEffect.Classes; 
public partial class DefaultBehavior
{
    public class EqualsTest : EqualityTestCase
    {
        public override bool Expected => false;
        public override object Factory1() => new Sample("Dave", 35);
        public override bool EqualsOperator(object value1, object value2) => (Sample) value1 == (Sample) value2;
        public override bool NotEqualsOperator(object value1, object value2) => (Sample) value1 != (Sample) value2;
    }
}