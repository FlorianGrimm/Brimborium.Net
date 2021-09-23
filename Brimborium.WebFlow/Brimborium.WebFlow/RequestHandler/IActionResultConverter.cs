using Brimborium.CodeFlow.Logic;

using Microsoft.AspNetCore.Mvc;
namespace Brimborium.CodeFlow.RequestHandler {
    public interface IActionResultConverter {
        ActionResult ConvertToActionResultVoid<TRequestResult>(
                ControllerBase controllerBase,
                RequestResult<TRequestResult> responseResultOfT
            )
            where TRequestResult : IServerResponse<ResultVoid>;

        ActionResult<TActionResult> ConvertToActionResultOfT<TRequestResult, TActionResult>
            (
            ControllerBase controllerBase,
            RequestResult<TRequestResult> responseResultOfT
            );
    }
}
