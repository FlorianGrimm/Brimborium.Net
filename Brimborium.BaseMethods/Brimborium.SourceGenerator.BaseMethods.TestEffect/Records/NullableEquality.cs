using TestEffect;

namespace TestEffect.Records; 
public partial class NullableEquality : EqualityTestCase
{
    public override object Factory1() => new Sample();
    public override bool EqualsOperator(object value1, object value2) => (Sample) value1 == (Sample) value2;
    public override bool NotEqualsOperator(object value1, object value2) => (Sample) value1 != (Sample) value2;
}