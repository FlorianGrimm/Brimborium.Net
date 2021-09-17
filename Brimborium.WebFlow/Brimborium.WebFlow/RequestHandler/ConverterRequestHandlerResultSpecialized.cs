using System;

using Microsoft.AspNetCore.Mvc;
namespace Brimborium.CodeFlow.RequestHandler {
    public abstract class ConverterRequestHandlerResultSpecialized<TRequestResult>
        : IRequestHandlerResultConverterSpecialized<TRequestResult>
        where TRequestResult : RequestHandlerResult {
        
        public ConverterRequestHandlerResultSpecialized() {
        }

        public virtual ActionResult<TResult> ConvertToActionResult<TResult>(ControllerBase controllerBase, RequestHandlerResult requestResult) {
            return this.Convert<TResult>(controllerBase, (TRequestResult)requestResult);
        }

        public abstract ActionResult<TResult> Convert<TResult>(ControllerBase controllerBase, TRequestResult requestResult);

        public Type GetSourceType() => typeof(TRequestResult);
    }
}
