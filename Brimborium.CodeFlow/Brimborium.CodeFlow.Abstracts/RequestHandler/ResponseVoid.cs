namespace Brimborium.CodeFlow.RequestHandler {
    public interface IResponseVoid
        : IResponse {
    }

    public sealed class ResponseVoid : IResponseVoid {
        private static ResponseVoid? _Instance;

        public static ResponseVoid GetInstance() => (_Instance ?? new ResponseVoid());

        public ResponseVoid() {
        }
    }
}
