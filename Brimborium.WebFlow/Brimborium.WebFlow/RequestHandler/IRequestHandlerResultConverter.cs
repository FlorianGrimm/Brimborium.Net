using System;

using Microsoft.AspNetCore.Mvc;
namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestHandlerResultConverter {

        ActionResult<TResult> ConvertTyped<TResponse, TResult>(
            ControllerBase controllerBase,
            RequestHandlerResult<TResponse> responseResult,
            Func<TResponse, TResult> extractValue);

        ActionResult ConvertActionResult<TResponse>(
            ControllerBase controllerBase,
            RequestHandlerResult<TResponse> responseResult,
            Func<TResponse, ActionResult>? convertToActionResult);
    }

    public interface IRequestHandlerResultConverterSpecialized {
        ActionResult ConvertToActionResult(ControllerBase controllerBase, RequestHandlerResult requestResult);
        Type GetTRequestHandlerResult();
    }

    public interface IRequestHandlerResultConverterSpecialized<TRequestHandlerResult>
        : IRequestHandlerResultConverterSpecialized
         where TRequestHandlerResult : RequestHandlerResult {
        ActionResult ConvertToActionResultSpecialized(ControllerBase controllerBase, TRequestHandlerResult requestResult);
    }

    public interface IRequestHandlerResultFormaterSpecialized {
        //ActionResult<TResult> ConvertTyped<TResult>(ControllerBase controllerBase, object resultValue);
        Type GetTResult();
    }

    public interface IRequestHandlerResultFormaterSpecialized<TResult> : IRequestHandlerResultFormaterSpecialized {
        ActionResult<TResult> ConvertTypedSpecialized(ControllerBase controllerBase, TResult resultValue);
    }
}
