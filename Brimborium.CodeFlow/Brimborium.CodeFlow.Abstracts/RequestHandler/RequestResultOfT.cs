using System.Diagnostics.CodeAnalysis;

namespace Brimborium.CodeFlow.RequestHandler {
    public sealed class RequestResult<TValue> {
        private readonly bool _ValueIsUsed;
        private readonly TValue? _Value;
        private readonly RequestResult? _Result;

        public RequestResult(TValue value) {
            this._Value = value;
            this._ValueIsUsed = true;
        }
        public RequestResult(RequestResult result) {
            this._Result = result;
            this._ValueIsUsed = false;
        }

        public bool TryGetValue([MaybeNullWhen(false)] out TValue value) {
            value = this._Value!;
            return (this._ValueIsUsed);
        }
        public TValue Value => (this._ValueIsUsed) ? (this._Value!) : throw new System.InvalidOperationException("Value is not set.");

        public bool TryGetResult([MaybeNullWhen(false)] out RequestResult result) {
            result = this._Result!;
            return !(this._ValueIsUsed);
        }

        public RequestResult Result => (!this._ValueIsUsed) ? (this._Result!) : new RequestHandlerResulttOK(this._Value, 200);

        public static implicit operator RequestResult<TValue>(TValue value) => new RequestResult<TValue>(value);
        public static implicit operator RequestResult<TValue>(RequestResult result) => new RequestResult<TValue>(result);
    }

}
