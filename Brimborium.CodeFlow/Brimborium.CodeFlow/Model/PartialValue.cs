namespace Brimborium.CodeFlow.Model {
    public static class PartialValue {
        public static void Set<T>(ref PartialValue<T> that, T value) {
            that = new PartialValue<T>(value);
        }
    }

    public struct PartialValue<T> {
        public readonly bool ValueIsSet;
        public readonly T? Value;
        public PartialValue(T value) {
            this.Value = value;
            this.ValueIsSet = true;
        }
    }
}
