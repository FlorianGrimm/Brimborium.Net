using System;

using Microsoft.AspNetCore.Mvc;
namespace Brimborium.CodeFlow.RequestHandler {
    public interface IRequestResultConverter {
        ActionResult ConvertToActionResult<TResponse>(
            ControllerBase controllerBase,
            RequestResult<TResponse> responseResultOfT,
            Func<TResponse, ActionResult>? convertToActionResult);

        ActionResult<TResult> ConvertToActionResultOfT<TResponse, TResult>(
            ControllerBase controllerBase,
            RequestResult<TResponse> responseResultOfT,
            Func<TResponse, TResult> extractValue);
    }

    public interface IRequestResultConverterSpecialized {
        ActionResult ConvertToActionResult(ControllerBase controllerBase, RequestResult requestResult);
        Type GetTRequestHandlerResult();
    }

    public interface IRequestResultConverterSpecialized<TRequestHandlerResult>
        : IRequestResultConverterSpecialized
         where TRequestHandlerResult : RequestResult {
        ActionResult ConvertToActionResultSpecialized(ControllerBase controllerBase, TRequestHandlerResult requestResult);
    }

    public interface IRequestResultFormaterSpecialized {
        //ActionResult<TResult> Format<TResult>(ControllerBase controllerBase, object resultValue);
        Type GetTResult();
    }

    public interface IRequestHandlerResultFormaterSpecialized<TResult> : IRequestResultFormaterSpecialized {
        ActionResult<TResult> FormatSpecialized(ControllerBase controllerBase, TResult resultValue);
    }
}
