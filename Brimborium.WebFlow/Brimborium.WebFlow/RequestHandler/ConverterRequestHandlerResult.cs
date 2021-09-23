using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Brimborium.CodeFlow.RequestHandler {
    public class ConverterRequestHandlerResult
        : IRequestResultConverter {
        private readonly IServiceProvider _ServiceProvider;
        private readonly Dictionary<Type, IRequestResultConverterSpecialized> _Converteres;
        private bool _ConvertersFilled;

        public ConverterRequestHandlerResult(IServiceProvider serviceProvider) {
            this._ServiceProvider = serviceProvider;
            this._Converteres = new Dictionary<Type, IRequestResultConverterSpecialized>();
        }

        public ActionResult ConvertToActionResult<TResponse>(ControllerBase controllerBase, RequestResult<TResponse> responseResultOfT, Func<TResponse, ActionResult>? convertToActionResult) {
            this.EnsureConverters();
            if (responseResultOfT.TryGetValue(out var responseValue)) {
                return ConvertTypedOk(responseValue);

            }
            var result = responseResultOfT.Result;

#warning TODO convert success value??

            var requestResult = responseResultOfT.Result;
            if (requestResult is RequestResultOK requestResultOK) {
                if ((requestResultOK.Value is TResponse responseOKValue)) {
                    return ConvertTypedOk(responseOKValue);
                }
            }

            if (this._Converteres.TryGetValue(requestResult.GetType(), out var converter)) {
                return converter.ConvertToActionResult(controllerBase, requestResult);
            }

            if (requestResult is RequestResultFailed requestResultFailed) {
                return ConvertFailed(controllerBase, requestResultFailed);
            }

            return controllerBase.BadRequest();

            ActionResult ConvertTypedOk(TResponse responseOKValue) {
                if (convertToActionResult is not null) {
                    ActionResult tResult = convertToActionResult(responseOKValue);
                    return tResult;
                } else {
                    return new OkObjectResult(responseOKValue);
                }
            }
        }

        private ActionResult ConvertFailed(ControllerBase controllerBase, RequestResultFailed requestResult) {
            if (requestResult is RequestResultException requestResultException) {
                if (requestResultException.Exception is not null && requestResultException.Status == -1) {
                    throw requestResultException.Exception;
                }
            }
            if (requestResult is RequestResultErrorDetails requestResultErrorDetails) {
                return controllerBase.Problem(
                    detail: requestResultErrorDetails.Detail,
                    instance: requestResultErrorDetails.Instance,
                    statusCode: requestResultErrorDetails.Status,
                    title: requestResultErrorDetails.Title,
                    type: requestResultErrorDetails.Type
                    );
            }
            if (requestResult is RequestResultForbidden requestResultForbidden) {
                return controllerBase.Forbid();
            }
            return controllerBase.BadRequest();

        }

        public ActionResult<TResult> ConvertToActionResultOfT<TResponse, TResult>(ControllerBase controllerBase, RequestResult<TResponse> requestHandlerResultT, Func<TResponse, TResult> extractValue) {
            this.EnsureConverters();
            if (requestHandlerResultT.TryGetValue(out var responseValue)) {
                return ConvertTypedOk(responseValue);
            }

#warning TODO convert success value??

            var requestResult = requestHandlerResultT.Result;
            if (requestResult is RequestResultOK requestResultOK) {
                if ((requestResultOK.Value is TResponse responseOKValue)) {
                    return ConvertTypedOk(responseOKValue);
                }
            }

            if (this._Converteres.TryGetValue(requestResult.GetType(), out var converter)) {
                return converter.ConvertToActionResult(controllerBase, requestResult);
            }

            if (requestResult is RequestResultForbidden requestResultForbidden) {
                return controllerBase.Forbid();
            }
            return controllerBase.BadRequest();

            ActionResult<TResult> ConvertTypedOk(TResponse responseOKValue) {
                TResult tResult = extractValue(responseOKValue);
                return tResult;
            }
        }

        private void EnsureConverters() {
            if (!_ConvertersFilled) {
                lock (this) {
                    if (!_ConvertersFilled) {
                        this._ConvertersFilled = true;
                        var services = this._ServiceProvider.GetServices<IRequestResultConverterSpecialized>();
                        foreach (var service in services) {
                            this._Converteres.Add(service.GetTRequestHandlerResult(), service);
                        }
                    }
                }
            }
        }
    }
}
