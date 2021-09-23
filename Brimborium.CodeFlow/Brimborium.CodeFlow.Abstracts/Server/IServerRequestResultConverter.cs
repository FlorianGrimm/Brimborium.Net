using Brimborium.CodeFlow.Logic;
using Brimborium.CodeFlow.RequestHandler;

using System;

namespace Brimborium.CodeFlow.Server {
    public interface IServerRequestResultConverter {
        RequestResult ConvertToServerResultVoid<TRequestResult>(
                RequestResult<TRequestResult> responseResultOfT
            )
            where TRequestResult : IServerResponse<ResultVoid>;

        RequestResult<TServerResult> ConvertToServerResultOfT<TRequestResult, TServerResult>(
            RequestResult<TRequestResult> responseResultOfT,
            Func<TRequestResult, TServerResult>? convertFunc=default
            );
    }

}
