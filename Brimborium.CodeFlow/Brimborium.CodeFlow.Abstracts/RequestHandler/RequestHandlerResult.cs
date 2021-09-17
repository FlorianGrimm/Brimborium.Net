using System;
using System.Diagnostics.CodeAnalysis;

namespace Brimborium.CodeFlow.RequestHandler {
    public class RequestHandlerResult {
        public RequestHandlerResult() {
        }
        public RequestHandlerResult(int status) {
            this.Status = status;
        }

        public int Status { get; init; }
    }

    public class RequestHandlerResulttOK : RequestHandlerResult {
        public RequestHandlerResulttOK(object? value, int? status = default) : base(status.GetValueOrDefault(200)) {
            this.Value = value;
        }

        public object? Value { get; init; }
    }

    public class RequestHandlerResultFailed : RequestHandlerResult {
        public RequestHandlerResultFailed(string message, string? scope=default, System.Exception? exception = default, int? status = default) : base(status.GetValueOrDefault(500)) {
            this.Message = message;
            this.Scope = scope;
            this.Exception = exception;
        }

        public string Message { get; init; }
        public string? Scope { get; }
        public Exception? Exception { get; init; }
    }

    public class RequestHandlerResult<TValue> {
        private readonly bool _ValueIsUsed;
        private readonly TValue? _Value;
        private readonly RequestHandlerResult? _Result;

        public RequestHandlerResult(TValue value) {
            this._Value = value;
            this._ValueIsUsed = true;
        }
        public RequestHandlerResult(RequestHandlerResult result) {
            this._Result = result;
            this._ValueIsUsed = false;
        }

        public bool TryGetValue([MaybeNullWhen(false)] out TValue value) {
            value = this._Value!;
            return (this._ValueIsUsed);
        }
        public TValue Value => (this._ValueIsUsed) ? (this._Value!) : throw new System.InvalidOperationException("Value is not set.");

        public bool TryGetResult([MaybeNullWhen(false)] out RequestHandlerResult result) {
            result = this._Result!;
            return !(this._ValueIsUsed);
        }

        public RequestHandlerResult Result => (!this._ValueIsUsed) ? (this._Result!) : new RequestHandlerResulttOK(this._Value, 200);

        public static implicit operator RequestHandlerResult<TValue>(TValue value) => new RequestHandlerResult<TValue>(value);
        public static implicit operator RequestHandlerResult<TValue>(RequestHandlerResult result) => new RequestHandlerResult<TValue>(result);
    }
}
