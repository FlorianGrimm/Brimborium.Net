using System;

using Microsoft.AspNetCore.Mvc;
namespace Brimborium.CodeFlow.RequestHandler {
    public abstract class RequestHandlerResultConverterSpecialized<TRequestHandlerResult>
        : IRequestResultConverterSpecialized<TRequestHandlerResult>
        where TRequestHandlerResult : RequestResult {

        public RequestHandlerResultConverterSpecialized() {
        }

        public virtual ActionResult ConvertToActionResult(ControllerBase controllerBase, RequestResult requestResult) {
            return this.ConvertToActionResultSpecialized(controllerBase, (TRequestHandlerResult) requestResult);
        }

        public abstract ActionResult ConvertToActionResultSpecialized(ControllerBase controllerBase, TRequestHandlerResult requestResult);

        public Type GetTRequestHandlerResult() => typeof(TRequestHandlerResult);
    }

    /*
    public abstract class RequestHandlerResultFormaterSpecialized<TResult>
        : IRequestHandlerResultFormaterSpecialized<TResult> {
        public RequestHandlerResultFormaterSpecialized() {
        }
        // public ActionResult<TResult1> ConvertTyped<TResult1>(ControllerBase controllerBase, TResult1 resultValue) {}
        public abstract ActionResult<TResult> ConvertTypedSpecialized(ControllerBase controllerBase, TResult resultValue);
        public Type GetTResult() => typeof(TResult);
    }
    */
}
