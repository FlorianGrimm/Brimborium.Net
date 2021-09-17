using System;

using Microsoft.AspNetCore.Mvc;
namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestHandlerResultConverterSpecialized {
        ActionResult<TResult> ConvertToActionResult<TResult>(ControllerBase controllerBase, RequestHandlerResult requestResult);
        Type GetSourceType();
    }

    public interface IRequestHandlerResultConverterSpecialized<TRequestResult>
        : IRequestHandlerResultConverterSpecialized
        where TRequestResult : RequestHandlerResult {
        ActionResult<TResult> Convert<TResult>(ControllerBase controllerBase, TRequestResult requestResult);
    }
}
