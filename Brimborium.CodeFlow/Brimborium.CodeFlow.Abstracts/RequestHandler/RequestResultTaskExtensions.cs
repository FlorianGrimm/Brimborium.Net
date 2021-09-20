using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.RequestHandler {
    public static class RequestResultTaskExtensions {

        /*
        public static async Task<RequestResult<O>> ExecuteRequestResultAsync<R, I, O>(
            Func<R, IRequestHandlerContext, CancellationToken, Task<I>> executeAsync,
            R request,
            IRequestHandlerContext context,
            CancellationToken cancellationToken,
            Func<I, O> convertOk, 
            int? status = default, 
            ILogger? logger = default) {
            try {
                I valueI = await executeAsync(request, context, cancellationToken);
                var valueO = convertOk(valueI);
                return new RequestResult<O>(valueO);
            } catch (System.Exception error) {
                return RequestResultException.CatchAndLog(error, status, logger);
            }
        }
        */

        public static async Task<RequestResult<T>> WrapRequestResult<T>(this Task<T> task, int? status = default, ILogger? logger = default) {
            try {
                T valueT = await task;
                return new RequestResult<T>(valueT);
            } catch (System.Exception error) {
                return RequestResultException.CatchAndLog(error, status, logger);
            }
        }

        public static async Task<RequestResult<O>> WrapRequestResult<I, O>(this Task<I> task, Func<I, O> convertOk, int? status = default, ILogger? logger = default) {
            try {
                I valueI = await task;
                var valueO = convertOk(valueI);
                return new RequestResult<O>(valueO);
            } catch (System.Exception error) {
                return RequestResultException.CatchAndLog(error, status, logger);
            }
        }
    }
}
