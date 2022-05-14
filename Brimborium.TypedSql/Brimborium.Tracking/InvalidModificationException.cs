namespace Brimborium.Tracking;

[Serializable]
public class InvalidModificationException : System.InvalidOperationException {
    public InvalidModificationException(string? message) : base(message) {
    }

    public InvalidModificationException(string? message, Exception? innerException) : base(message, innerException) {
    }

    protected InvalidModificationException(
        SerializationInfo serializationInfo, 
        StreamingContext streamingContext
        ) : base(serializationInfo, streamingContext) {
    }

    public override void GetObjectData(
        SerializationInfo info, 
        StreamingContext context) {
        base.GetObjectData(info, context);
    }
}