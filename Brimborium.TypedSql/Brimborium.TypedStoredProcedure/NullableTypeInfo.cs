
using System;

namespace Brimborium.TypedStoredProcedure {
    public class NullableTypeInfo {
        public NullableTypeInfo CreateAsNullable(Type givenType) {
            var underlyingType = Nullable.GetUnderlyingType(givenType);
            if (underlyingType is object) {
                return new NullableTypeInfo(givenType);
            } else {
                var nullableType = typeof(Nullable<>).MakeGenericType(new[] { givenType });
                return new NullableTypeInfo(nullableType);
            }
            //
        }
        public NullableTypeInfo(Type givenType) {
            this.GivenType = givenType;
            this.UnderlyingType = Nullable.GetUnderlyingType(this.GivenType);
            this.NotNullableType = this.UnderlyingType ?? this.GivenType;
        }

        public bool IsNullableType => this.UnderlyingType is object;
        public Type GivenType { get; }
        public Type? UnderlyingType { get; }
        public Type NotNullableType { get; }
    }

}
