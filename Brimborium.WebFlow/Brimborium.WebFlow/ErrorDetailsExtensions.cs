using Microsoft.AspNetCore.Mvc;

namespace Brimborium.CodeFlow.RequestHandler {
    public static class ErrorDetailsExtensions {
        public static ProblemDetails ConvertToProblemDetails(this RequestResultErrorDetails source) {
            var target = new ProblemDetails() {
                Type = source.Type,
                Title = source.Title,
                Status = source.Status,
                Detail = source.Detail,
                Instance = source.Instance
            };

            foreach (var kvp in source.Extensions) {
                target.Extensions[kvp.Key] = kvp.Value;
            }
            return target;
        }

        public static RequestResultErrorDetails ConvertToProblemDetails(this ProblemDetails source) {
            var target  = new RequestResultErrorDetails() {
                Type = source.Type,
                Title = source.Title,
                Status = source.Status,
                Detail = source.Detail,
                Instance = source.Instance
            };

            foreach (var kvp in source.Extensions) {
                target.Extensions[kvp.Key] = kvp.Value;
            }
            return target;
        }
    }
}
