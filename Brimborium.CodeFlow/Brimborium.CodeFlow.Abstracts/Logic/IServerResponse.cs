namespace Brimborium.CodeFlow.Logic {
    public interface IServerResponseVoid { }
    public class ServerResponseVoid : IServerResponseVoid {
        public ServerResponseVoid() {
        }
    }
    public interface IServerResponse<T> { T Value { get; } }
}
