namespace Brimborium.CodeFlow.RequestHandler {
    // Code Gen
    public interface IResponse { 
    }
    
    public interface IResponse<T> : IResponse {
        T Value { get; } 
    }
}
