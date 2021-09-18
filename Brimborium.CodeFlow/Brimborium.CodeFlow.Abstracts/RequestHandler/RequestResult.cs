using System;
using System.Collections.Generic;

namespace Brimborium.CodeFlow.RequestHandler {
    public class RequestResult {
        public RequestResult() {
        }
        public RequestResult(int? status) {
            this.Status = status;
        }

        public int? Status { get; init; }
    }

    public class RequestHandlerResulttOK : RequestResult {
        public RequestHandlerResulttOK(object? value) : base(200) {
            this.Value = value;
        }

        public RequestHandlerResulttOK(object? value, int? status) : base(status) {
            this.Value = value;
        }

        public object? Value { get; init; }
    }

    public class RequestResultFailed : RequestResult {
        public RequestResultFailed() {
        }

        public RequestResultFailed(int? status) : base(status) {
        }
    }

    public class RequestResultException : RequestResultFailed {
        public RequestResultException() {
        }

        public RequestResultException(int? status) : base(status) {
        }

        public RequestResultException(int? status, Exception? exception) : base(status) {
            this.Exception = exception;
        }

        public bool Rethrow { get; init; }
        public Exception? Exception { get; init; }
    }

    public sealed class RequestResultErrorDetails : RequestResultFailed {
        public RequestResultErrorDetails() : base(400) { }
        public RequestResultErrorDetails(int? status) : base(status) { }

        public string? Type { get; init; }

        public string? Title { get; init; }

        //public int? Status { get; init }

        public string? Detail { get; init; }

        public string? Instance { get; init; }

        public IDictionary<string, object?> Extensions { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);
    }

    public sealed class RequestResultForbidden : RequestResultFailed {
        public RequestResultForbidden() : base(403) { }
    }
}
