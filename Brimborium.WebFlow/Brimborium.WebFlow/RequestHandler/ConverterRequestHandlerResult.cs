using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
namespace Brimborium.CodeFlow.RequestHandler {
    public class ConverterRequestHandlerResult
        : IRequestHandlerResultConverter {
        private readonly IServiceProvider _ServiceProvider;
        private readonly Dictionary<Type, IRequestHandlerResultConverterSpecialized> _Converteres;
        private bool _ConvertersFilled;

        public ConverterRequestHandlerResult(IServiceProvider serviceProvider) {
            this._ServiceProvider = serviceProvider;
            this._Converteres = new Dictionary<Type, IRequestHandlerResultConverterSpecialized>();
        }

        public ActionResult ConvertActionResult<TResponse>(ControllerBase controllerBase, RequestHandlerResult<TResponse> requestHandlerResultT, Func<TResponse, ActionResult>? convertToActionResult) {
            this.EnsureConverters();
            if (requestHandlerResultT.TryGetValue(out var responseValue)) {
                if (convertToActionResult is not null) {
                    return convertToActionResult(responseValue);
                }
            }
            var result = requestHandlerResultT.Result;

#warning TODO convert success value??

            var requestResult = requestHandlerResultT.Result;
            if (requestResult is RequestHandlerResulttOK requestResultOK) {
                if ((requestResultOK.Value is TResponse responseOKValue)) {
                    return ConvertTypedOk(responseOKValue);
                }
            }

            if (this._Converteres.TryGetValue(requestResult.GetType(), out var converter)) {
                return converter.ConvertToActionResult(controllerBase, requestResult);
            }

            if (requestResult is RequestHandlerResultFailed requestResultFailed) {
                if (requestResultFailed.Exception is not null && requestResultFailed.Status == -1) {
                    throw requestResultFailed.Exception;
                }
                return controllerBase.Problem(
                    detail: requestResultFailed.Message,
                    statusCode: requestResultFailed.Status,
                    title: requestResultFailed.Scope
                    );
            }
            return controllerBase.BadRequest();

            ActionResult ConvertTypedOk(TResponse responseOKValue) {
                ActionResult tResult = extractValue(responseOKValue);
                return tResult;
            }
        }

        public ActionResult<TResult> ConvertTyped<TResponse, TResult>(ControllerBase controllerBase, RequestHandlerResult<TResponse> requestHandlerResultT, Func<TResponse, TResult> extractValue) {
            this.EnsureConverters();
            if (requestHandlerResultT.TryGetValue(out var responseValue)) {
                return ConvertTypedOk(responseValue);
            }
        
#warning TODO convert success value??
            
            var requestResult = requestHandlerResultT.Result;
            if (requestResult is RequestHandlerResulttOK requestResultOK) {
                if ((requestResultOK.Value is TResponse responseOKValue)) {
                    return ConvertTypedOk(responseOKValue);
                }
            }

            if (this._Converteres.TryGetValue(requestResult.GetType(), out var converter)) {
                return converter.ConvertToActionResult(controllerBase, requestResult);
            }

            if (requestResult is RequestHandlerResultFailed requestResultFailed) {
                if (requestResultFailed.Exception is not null && requestResultFailed.Status == -1) {
                    throw requestResultFailed.Exception;
                }
                return controllerBase.Problem(
                    detail: requestResultFailed.Message,
                    statusCode: requestResultFailed.Status,
                    title: requestResultFailed.Scope
                    );
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
                        var services = this._ServiceProvider.GetServices<IRequestHandlerResultConverterSpecialized>();
                        foreach (var service in services) {
                            this._Converteres.Add(service.GetSourceType(), service);
                        }
                    }
                }
            }
        }
    }
}
