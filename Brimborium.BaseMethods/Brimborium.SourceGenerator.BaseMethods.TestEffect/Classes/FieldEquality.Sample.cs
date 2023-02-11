namespace TestEffect.Classes;

public partial class FieldEquality {
    [Equatable]
    public partial class Sample {
        public Sample(string[] addresses) {
            _addresses = addresses;
        }

        [OrderedEquality] readonly string[] _addresses;

        public override string ToString()
            => $"Sample (_addresses: {_addresses})";
    }
}