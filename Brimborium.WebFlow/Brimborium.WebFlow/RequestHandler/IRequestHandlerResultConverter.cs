using System;

using Microsoft.AspNetCore.Mvc;
namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestHandlerResultConverter {

        ActionResult<TResult> Convert<TResponse, TResult>(
            ControllerBase controllerBase,
            RequestHandlerResult<TResponse> responseResult,
            Func<TResponse, TResult> extractValue);
    }
}
