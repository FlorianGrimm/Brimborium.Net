using Microsoft.Extensions.Logging;

using System;

namespace Brimborium.CodeFlow.RequestHandler {
    public class RequestResultException : RequestResultFailed {
        public static RequestResultException CatchAndLog(Exception error, int? status=default, ILogger? logger = default) {
            if (logger is not null) {
                if (error is AggregateException aggregateException) {
                    /*
                        var sb = new StringBuilder();
                        aggregateException.Handle((innerError) => {
                            sb.AppendLine(innerError.Message);
                            return true;
                        });
                    */
                    aggregateException.Handle(IgnoreException);
                    logger.LogError(error, "Error");
                } else {
                    logger.LogError(error, "Error");
                }
            } else {
                if (error is AggregateException aggregateException) {
                    aggregateException.Handle(IgnoreException);
                }
            }
            return new RequestResultException(status, error);

            static bool IgnoreException(System.Exception innerError) { return true; }
        }

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
}
