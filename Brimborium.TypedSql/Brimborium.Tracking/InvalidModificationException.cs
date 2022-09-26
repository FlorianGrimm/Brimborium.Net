namespace Brimborium.Tracking;

[Serializable]
public class InvalidModificationException : System.InvalidOperationException {
    private string? _Property;
    private string? _PrimaryKey;
    private string? _Type;

    public string? Property { get => _Property; }
    public string? PrimaryKey { get => _PrimaryKey; }
    public string? Type { get => _Type; }

    public InvalidModificationException(string? message) : base(message) {
    }

    public InvalidModificationException(string message, string property, string primaryKey, string? type) : base(message) {
        this._Property = property;
        this._PrimaryKey = primaryKey;
        this._Type = type;
    }

    public InvalidModificationException(string? message, Exception? innerException) : base(message, innerException) {
    }

    protected InvalidModificationException(
        SerializationInfo serializationInfo,
        StreamingContext streamingContext
        ) : base(serializationInfo, streamingContext) {
        serializationInfo.AddValue(nameof(this.Property), this.Property ?? string.Empty);
        serializationInfo.AddValue(nameof(this.PrimaryKey), this.PrimaryKey ?? string.Empty);
        serializationInfo.AddValue(nameof(this.Type), this.Type ?? string.Empty);
    }

    public override void GetObjectData(
        SerializationInfo info,
        StreamingContext context) {
        base.GetObjectData(info, context);
        try {
            this._Property = info.GetString(nameof(this.Property)) ?? String.Empty;
        } catch {
        }
        try {
            this._PrimaryKey = info.GetString(nameof(this.PrimaryKey)) ?? String.Empty;
        } catch {
        }
        try {
            this._Type = info.GetString(nameof(this.Type)) ?? String.Empty;
        } catch {
        }
    }
}