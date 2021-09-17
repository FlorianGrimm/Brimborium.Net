using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
namespace Brimborium.CodeFlow.RequestHandler {
    public class ConverterRequestHandlerResult : IRequestHandlerResultConverter {
        private readonly IServiceProvider _ServiceProvider;
        private readonly Dictionary<Type, IRequestHandlerResultConverterSpecialized> _Converteres;
        private bool _ConvertersFilled;

        public ConverterRequestHandlerResult(IServiceProvider serviceProvider) {
            this._ServiceProvider = serviceProvider;
            this._Converteres = new Dictionary<Type, IRequestHandlerResultConverterSpecialized>();
        }

        public ActionResult<TResult> Convert<TResponse, TResult>(ControllerBase controllerBase, RequestHandlerResult<TResponse> response, Func<TResponse, TResult> extractValue) {
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
            if (this._Converteres.TryGetValue(typeof(RequestHandlerResult), out var converter)) {
                return converter.ConvertToActionResult<TResult>(controllerBase, response.Result);
            }
#warning TODO convert success value??
            if (response.TryGetValue(out var responseValue)) {
                return extractValue(responseValue);
            }
            if (response.TryGetResult(out var result)) {
                if (result is RequestHandlerResulttOK requestResultOK) {
                    if (requestResultOK.Value is TResponse responseOKValue) {
                        return extractValue(responseOKValue);
                    }
                }
                if (result is RequestHandlerResultFailed requestResultFailed) {
                    if (requestResultFailed.Exception is not null && requestResultFailed.Status == -1) {
                        throw requestResultFailed.Exception;
                    }
                    return controllerBase.Problem(
                        detail: requestResultFailed.Message,
                        statusCode: requestResultFailed.Status,
                        title: requestResultFailed.Scope
                        );
                }
            }
            return controllerBase.BadRequest();
        }
    }
}
