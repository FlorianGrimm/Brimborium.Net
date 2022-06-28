namespace Brimborium.Tracking;

[Serializable]
public class InvalidModificationException : System.InvalidOperationException {
    public string? Property { get; private set; }
    public string? PrimaryKey { get; private set; }

    public InvalidModificationException(string? message) : base(message) {
    }

    public InvalidModificationException(string message, string property, string primaryKey) : base(message) {
        this.Property = property;
        this.PrimaryKey = primaryKey;
    }

    public InvalidModificationException(string? message, Exception? innerException) : base(message, innerException) {
    }

    protected InvalidModificationException(
        SerializationInfo serializationInfo, 
        StreamingContext streamingContext
        ) : base(serializationInfo, streamingContext) {
        serializationInfo.AddValue(nameof(this.Property), this.Property);
        serializationInfo.AddValue(nameof(this.PrimaryKey), this.PrimaryKey);
    }

    public override void GetObjectData(
        SerializationInfo info, 
        StreamingContext context) {
        base.GetObjectData(info, context);
        this.Property = info.GetString(nameof(this.Property)) ?? String.Empty;
        this.PrimaryKey = info.GetString(nameof(this.PrimaryKey)) ?? String.Empty;
    }
}